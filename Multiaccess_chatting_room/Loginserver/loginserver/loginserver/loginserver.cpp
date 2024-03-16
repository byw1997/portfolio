#pragma once

#include <iostream>
#include <string>
#include <vector>
#include <WinSock2.h>
#include <pqxx/pqxx>
#include <thread>
#include <random>
#include <ctime>
#include <ws2tcpip.h>
#include <queue>

#define PORT 20000
#define PACKET_SIZE 1024
#define MAX_USERS_ONLINE 10//최대 동시 접속 10인

using namespace std;

time_t timer;
struct tm t;

int online_user_number = 0;
int online_users[MAX_USERS_ONLINE];//접속중인 유저들의 id
string user_nicknames[MAX_USERS_ONLINE];
SOCKET sockd;//socket descriptor
SOCKET client_sockets[MAX_USERS_ONLINE];//유저와 tcp연결할 소켓
SOCKADDR_IN clients[MAX_USERS_ONLINE] = { 0 };
int client_size[MAX_USERS_ONLINE];
vector<string> last_10_messages(10,"");
queue<int> empty_user_slot;
pqxx::connection connectionObject("host=localhost port=5432 dbname=serverpj user=dbadmin password=dbpassword");



vector<string> split_by_blank(string s) {
    vector<string> ans;
    string temp;

    for (int i = 0; i < s.size(); i++) {
        if (s[i] == ' ') {
            ans.push_back(temp);
            temp = "";
        }
        else {
            temp += s[i];
        }
    }
    if(temp.size() != 0){
        ans.push_back(temp);
    }
    return ans;
    
}

int get_user_id(string id, string pw) {
    pqxx::work worker(connectionObject);
    try {
        pqxx::result response = worker.exec_params("SELECT user_id FROM users WHERE username = $1 and userpw = $2", id, pw);
        if(!response.empty())
            return response[0][0].as<int>();
        else {
            return -1;
        }
    }
    catch (const exception& e) {//에러
        cerr << e.what() << endl;
        return -2;
    }
    
}

int idgen() {
    random_device rd;
    mt19937 gen(rd());
    uniform_int_distribution<int> distribution(100000, 999999);

    return distribution(gen);
}

int sign_up(string id, string pw, string nickname) {
    pqxx::work worker(connectionObject);
    int new_user_id;
    try {
        pqxx::result response = worker.exec_params("select count(distinct username) from users where username = $1", id);
        if (response[0][0].as<int>()> 0) {
            return -2;
        }
        response = worker.exec_params("select count(distinct nickname) from userdata where nickname = $1", nickname);
        if (response[0][0].as<int>() > 0) {
            return -3;
        }
        new_user_id = idgen();//userid 생성
        response = worker.exec_params("select count(distinct user_id) from users where user_id = $1", new_user_id);
        while (response[0][0].as<int>() > 0) {//겹치는지 확인, 겹치면 재생성
            new_user_id = idgen();
            response = worker.exec_params("select count(distinct user_id) from users where user_id = $1", new_user_id);
        }
        try{//정보 db에 생성
            worker.exec_params("insert into users values($1,$2,$3)", new_user_id,id,pw);
            worker.exec_params("insert into userdata values($1,$2)", new_user_id,nickname);
            worker.exec_params("insert into permissions values($1,1)", new_user_id);
            worker.commit();
        }
        catch (const exception& e) {
            cerr << e.what() << endl;
            return -1;
        }
        return 0;
    }
    catch (const exception& e) {//에러
        cerr << e.what() << endl;
        return -1;
    }
}

int say(vector<string> commands, string msg, int cnum) {
    for (int i = 1; i < commands.size(); i++) {
        if (i > 1) {
            msg += ' ';
        }
        msg += commands[i];
    }
    if (msg.size() > 0) {
        time(&timer);
        localtime_s(&t, &timer);
        msg = "[" + to_string(t.tm_year + 1900) + "-" + to_string(t.tm_mon + 1) + "-" + to_string(t.tm_mday) + " "
            + to_string(t.tm_hour) + ":" + to_string(t.tm_min) + "]" + user_nicknames[cnum] + ">> " + msg;
        if (last_10_messages.size() >= 10) {
            last_10_messages.erase(last_10_messages.begin());
        }
        last_10_messages.push_back(msg);
        for (int i = 0; i < MAX_USERS_ONLINE; i++) {
            if (online_users[i] != 0) {
                send(client_sockets[i], msg.c_str(), msg.size(), 0);
            }
        }
        return 0;
    }
    else {
        return -1;
    }
}

int whisper(vector<string> commands, string msg, int cnum) {
    if (commands.size() == 1) {
        msg = "Incomplete format.";
        send(client_sockets[cnum], msg.c_str(), msg.size(), 0);
        return -2;
    }
    else {
        string target_nickname = commands[1];
        int target_num = -1;
        for (int i = 0; i < MAX_USERS_ONLINE; i++) {
            if (user_nicknames[i] == target_nickname) {
                target_num = i;
                break;
            }
        }
        for (int i = 2; i < commands.size(); i++) {
            if (i > 2) {
                msg += ' ';
            }
            msg += commands[i];
        }
        if (target_num != -1) {
            if (msg.size() > 0) {
                time(&timer);
                localtime_s(&t, &timer);
                msg = "[" + to_string(t.tm_year + 1900) + "-" + to_string(t.tm_mon + 1) + "-" + to_string(t.tm_mday) + " "
                    + to_string(t.tm_hour) + ":" + to_string(t.tm_min) + "]" + "(Whisper)" + user_nicknames[cnum] + ">> " + msg;
                cout << msg << endl;
                send(client_sockets[target_num], msg.c_str(), msg.size(), 0);
            }
            return 0;
        }
        else {
            msg = "No such user";
            send(client_sockets[cnum], msg.c_str(), msg.size(), 0);
            return -1;
        }
    }
}

void disconnect_client(int cnum) {
    online_user_number--;
    online_users[cnum] = 0;
    closesocket(client_sockets[cnum]);//종료된 클라이언트 정보 제거
    user_nicknames[cnum] = "";
    empty_user_slot.push(cnum);
    return;
}

void recv_from_client(SOCKET& s, int cnum) {
    cout << "New Thread" << endl;
    char buf[PACKET_SIZE];
    char ans[PACKET_SIZE];
    int userid;
    while (1) {
        ZeroMemory(buf, PACKET_SIZE);
        if (recv(s, buf, PACKET_SIZE, 0) <= 0) {//로그인 화면 명령어 전달받음
            disconnect_client(cnum);
            cout << "Client #" + to_string(cnum) + " disconnected" << endl;
            return;
        }
        string command(buf);
        if (command == "login") {//로그인화면에서 로그인 전달받음

            ZeroMemory(buf, PACKET_SIZE);
            if (recv(s, buf, PACKET_SIZE, 0) == -1) {//id 전달받음
                return;
            }
            string inputid(buf);

            ZeroMemory(buf, PACKET_SIZE);
            if (recv(s, buf, PACKET_SIZE, 0) == -1) {//pw 전달받음
                return;
            }
            string inputpw(buf);
            userid = get_user_id(inputid, inputpw);//로그인 시도
            if (userid == -1) {
                string noaccount = "NOACC";
                strcpy_s(ans, noaccount.c_str());
                send(client_sockets[cnum], ans, strlen(ans), 0);
                cout << "Client #" << cnum << " tried to login with inappropriate name or pw." << endl;
                cout << "Error occured!" << endl;

            }
            else if (userid == -2) {
                string unknown = "UKN";
                strcpy_s(ans, unknown.c_str());
                send(client_sockets[cnum], ans, strlen(ans), 0);
                cout << "Client #" << cnum << " got unknown error." << endl;
                cout << "Error occured!" << endl;

            }
            else {
                for (int i = 0; i < MAX_USERS_ONLINE; i++) {
                    if (online_users[i] == userid) {
                        string ext = "EXISTING";
                        strcpy_s(ans, ext.c_str());
                        send(client_sockets[cnum], ans, strlen(ans), 0);
                        cout << "Client #" << cnum << " tried to login account already logged in." << endl;
                        cout << "Error occured!" << endl;
                        continue;
                    }
                }
                //
                pqxx::work worker(connectionObject);
                pqxx::result response = worker.exec_params("SELECT nickname FROM userdata WHERE user_id = $1", userid);
                string loggedin = "SCS";
                strcpy_s(ans, loggedin.c_str());
                online_users[cnum] = userid;//성공
                
                send(client_sockets[cnum], ans, strlen(ans), 0);
                cout << "Client #" << cnum << " successfully logged in." << endl;
                string nickname;
                response[0][0].to(nickname);
                user_nicknames[cnum] = nickname;
                time(&timer);
                localtime_s(&t, &timer);
                string welcome = "[" + to_string(t.tm_year + 1900) + "-" + to_string(t.tm_mon + 1) + "-" + to_string(t.tm_mday) + " "
                    + to_string(t.tm_hour) + ":" + to_string(t.tm_min) + "]" + "Welcome, " + nickname;
                
                send(client_sockets[cnum], welcome.c_str(), welcome.size(), 0);
                string oldmessage;
                for (int i = 0; i < 10; i++) {
                    oldmessage = last_10_messages[i]+'\n';
                    if (oldmessage != "\n") {
                        send(client_sockets[cnum], oldmessage.c_str(), oldmessage.size(), 0);
                    }
                }

                break;
            }
        }
        else if (command == "signup") {//로그인화면에서 회원가입 전달받음
            ZeroMemory(buf, PACKET_SIZE);
            if (recv(s, buf, PACKET_SIZE, 0) == -1) {//id
                return;
            }
            string inputid(buf);

            ZeroMemory(buf, PACKET_SIZE);
            if (recv(s, buf, PACKET_SIZE, 0) == -1) {//pw
                return;
            }
            string inputpw(buf);

            ZeroMemory(buf, PACKET_SIZE);
            if (recv(s, buf, PACKET_SIZE, 0) == -1) {//pw
                return;
            }
            string nickname(buf);
            string ret;
            try {
                int signuphandler = sign_up(inputid, inputpw, nickname);
                if (signuphandler == -1) {
                    cout << "sign up failed!" << endl;
                    ret = "Failed!";
                }
                else if (signuphandler == -2) {
                    ret = "Existing ID";
                }
                else if (signuphandler == -3) {
                    ret = "Existing Nickname";
                }
                else if(signuphandler == 0)
                    ret = "Now login!";
                send(client_sockets[cnum], ret.c_str(), ret.size(), 0);
            }
            catch (const exception& e) {
                cerr << e.what() << endl;
                return;
            }
        }
        else if (command == "exit") {
            disconnect_client(cnum);
            return;
        }
    }
    while (1) {
        ZeroMemory(buf, PACKET_SIZE);
        if (recv(s, buf, PACKET_SIZE, 0) == -1) {//연결 종료
            disconnect_client(cnum);
            cout << "Client #" << cnum << " exited" << endl;
            break;
        }

        string command(buf);
        vector<string> commands;
        string msg="";
        commands = split_by_blank(command);
        if (commands[0] == "say") {//int say로 encapsulation
            say(commands, msg, cnum);
        }
        else if (commands[0] == "whisper") {//int whisper로 encapsulation
            whisper(commands, msg, cnum);
        }
        else if (commands[0] == "users") {
            time(&timer);
            localtime_s(&t, &timer);
            msg += "[" + to_string(t.tm_year + 1900) + "-" + to_string(t.tm_mon + 1) + "-" + to_string(t.tm_mday) + " "
                + to_string(t.tm_hour) + ":" + to_string(t.tm_min) + "]" + "Current online users:";
            for (int i = 0; i < MAX_USERS_ONLINE; i++) {
                if (online_users[i] != 0) {
                    msg += " " +user_nicknames[i];
                }
            }
            msg += "\n";
            send(client_sockets[cnum], msg.c_str(), msg.size(), 0);
        }
    }
}

void connect_client() {//client와 연결하는 함수
    cout << "New Thread" << endl;
    char client_num[10] = { 0 }; //클라이언트정수값을 문자열로 저장하기위한 저장용 변수
    int client_num_counter = 0;
    while (1) {
        if (online_user_number < MAX_USERS_ONLINE) {
            int slot = empty_user_slot.front();
            empty_user_slot.pop();
            cout << "slot " << slot << endl;

            client_size[slot] = sizeof(clients[slot]);
            client_sockets[slot] = accept(sockd, (SOCKADDR*)&clients[slot], &client_size[slot]);
            online_user_number++;

            if (client_sockets[slot] == INVALID_SOCKET) {
                cout << "accept error";
                closesocket(client_sockets[slot]);
                closesocket(sockd);
                WSACleanup();
                return;
            }

            cout << "Client #" << slot << " Joined!" << endl; // 클라이언트 연결감지
            ZeroMemory(client_num, sizeof(client_num)); // 저장용변수 내용초기화
            _itoa_s(slot, client_num, 10); // client_num_counter;의 정수값을 client_num에다가 10진수로 저장
            if (client_num[9] != '\0') {
                cout << "Error occurred" << endl;
                return;
            }
            send(client_sockets[slot], client_num, strlen(client_num) + 1, 0); // 클라이언트번호 전송
            thread(recv_from_client, ref(client_sockets[slot]), slot).detach(); // 해당클라이언트 쓰레드생성
        }
        else {
            this_thread::sleep_for(chrono::seconds(3));
            continue;
        }

        
    }
}

int main()//게임 서버와 tcp로 연결하고 클라이언트의 tcp 연결을 대기하다가 로그인 요청이 들어오면 tcp로 연결하고 아이디/비번을 받아 유저 id를 찾는다. 정보가 존재하면 id를 현재 접속중인 유저 명단에 추가하고 유저에게 게임 서버 ip와 port를, 게임 서버에 유저 ip와 port를 전달한다.  
{
    string input;//command input
    
    vector<int> users(MAX_USERS_ONLINE,-1);
    WSADATA wsadata;
    pqxx::work worker(connectionObject);
    worker.exec_params("create table if not exists users(user_id int not null primary key, username varchar not null, userpw varchar not null);");
    worker.exec_params("create table if not exists userdata(user_id int not null primary key, nickname varchar not null);");
    worker.exec_params("create table if not exists permissions(user_id int not null primary key, plevel int not null)");
   /* worker.exec_params("create table if not exists rooms(room varchar not null primary key, privacy int not null, owner varchar not null);");
    worker.exec_params("create table if not exists enteredrooms(user_id int not null not null primary key, room_list varchar[]);");
    worker.exec_params("create table if not exists messages(room varchar not null primary key, user_id int not null, year int not null, month int not null,date int not null, hour int not null, minute int not null,  second int not null, message varchar not null)");*/
    worker.commit();
    for (int i = 0; i < MAX_USERS_ONLINE; i++) {
        empty_user_slot.push(i);
    }

    if (WSAStartup(MAKEWORD(2, 2), &wsadata)) {
        cout << "WSADATA ERROR" << endl;;
        connectionObject.close();
        return 0;
    }
    sockd = socket(AF_INET, SOCK_STREAM, 0);
    if (sockd == INVALID_SOCKET) {
        cout << "socket error"<<endl;
        closesocket(sockd);
        WSACleanup();
        connectionObject.close();
        return 0;
    }
    SOCKADDR_IN addr = {};
    addr.sin_family = AF_INET;
    addr.sin_port = htons(PORT);//set port number 포트 넘버 설정
    addr.sin_addr.s_addr = htonl(INADDR_ANY);//모든 address로부터의 연결 수락

    if (::bind(sockd, (SOCKADDR*)&addr, sizeof(addr))) {
        cout << "bind error "<<errno<<endl;
        closesocket(sockd);
        WSACleanup();
        connectionObject.close();
        return 0;
    }
    if (listen(sockd, SOMAXCONN)) {
        cout << "listen error"<<endl;
        closesocket(sockd);
        WSACleanup();
        connectionObject.close();
        return 0;
    }
    thread(connect_client).detach();//클라이언트 연결 스레드 생성

    while (1) {//get commands
        cout << "Enter commands" << endl;
        getline(cin,input);//add command handler
        cout << input << " entered" << endl;
        transform(input.begin(), input.end(), input.begin(), ::tolower);
        if (input=="exit") {
            cout << "Exit server" << endl;
            closesocket(sockd);
            WSACleanup();
            connectionObject.close();
            return 0;
        }
        else if (input == "users") {
            for (int i = 0; i < MAX_USERS_ONLINE; i++) {
                if (online_users[i] != 0) {
                    cout<<"Client #" << i << " is " << user_nicknames[i] <<" and "<< online_users[i] << " user" << endl;
                }
            }
        }
        else {
            cout << input << " is not defined." << endl;
        }
    }
}
