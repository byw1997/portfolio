#pragma once

#include <WinSock2.h>
#include <thread>
#include <iostream>
#include <string>
#include <algorithm>
#include <WS2tcpip.h>
#include <ctime>

using namespace std;

#define PACKET_SIZE 1024
#define PORT 20000 //destination port
#define IP_ADDR "127.0.0.1"//destination ip

char buf[PACKET_SIZE];
time_t timer;
struct tm t;
int target_name_loc;
int msg_loc;
string helpmessage = "say <message>: Send your <message> to everyone in public chatroom\n"
"whisper <target> <message>: Send your <message> to <target>\n"
"users : Check current online users\n"
"exit : Exit program\n";

int login(SOCKET& s) {

    string inputid;
    string inputpw;

    send(s, "login", sizeof("login"), 0);

    cout << "Enter username: " << endl;
    getline(cin, inputid);
    send(s, inputid.c_str(), inputid.size(), 0);

    cout << "Enter password: " << endl;
    getline(cin, inputpw);
    send(s, inputpw.c_str(), inputpw.size(), 0);

    ZeroMemory(buf, PACKET_SIZE);
    if (recv(s, buf, PACKET_SIZE, 0) == -1) {
        return -1;
    }
    string msg(buf);
    if (msg == "SCS") {
        return 0;
    }
    else if (msg == "NOACC") {
        cout << "No such account" << endl;
        return -1;
    }
    else if (msg == "UKN") {
        cout << "Unknown error" << endl;
        return -1;
    }
    else if (msg == "EXISTING") {
        cout << "This account has already logged in!" << endl;
        return -1;
    }
}

int signup(SOCKET& s) {
    

    string inputid;
    string inputpw;
    string nickname;

    send(s, "signup", sizeof("signup"), 0);

    cout << "Enter username: " << endl;
    getline(cin, inputid);
    send(s, inputid.c_str(), inputid.size(), 0);

    cout << "Enter password: " << endl;
    getline(cin, inputpw);
    send(s, inputpw.c_str(), inputpw.size(), 0);

    cout << "Enter your nickname: " << endl;
    getline(cin, nickname);
    send(s, nickname.c_str(), nickname.size(), 0);

    ZeroMemory(buf, PACKET_SIZE);
    if (recv(s, buf, PACKET_SIZE, 0) == -1) {
        return -1;
    }
    cout << buf << endl;

}

void recv_from_server(SOCKET& s) {//일단 메세지 표시만 함
    char recv_buf[PACKET_SIZE];
    while (1) {
        ZeroMemory(recv_buf, PACKET_SIZE);
        if (recv(s, recv_buf, PACKET_SIZE, 0)<=0) {//명령어 전달받음
            cout << "Disconnected" << endl;
            closesocket(s);
            WSACleanup;
            return;
        }
        string msg(recv_buf);
        cout << "\n"<<recv_buf << endl;
    }
    return;
}

int main()
{
    SOCKET sockd;
    string input;
    
    WSADATA wsadata;
    if (WSAStartup(MAKEWORD(2, 2), &wsadata)) {
        cout << "WSADATA ERROR";
        return 0;
    }
    sockd = socket(AF_INET, SOCK_STREAM, 0);
    if (sockd == INVALID_SOCKET) {
        cout << "socket error";
        closesocket(sockd);
        WSACleanup();
        return 0;
    }
    SOCKADDR_IN addr = {};
    addr.sin_family = AF_INET;
    addr.sin_port = htons(PORT);
    if (inet_pton(AF_INET, IP_ADDR, &addr.sin_addr.s_addr) != 1) {
        std::cerr << "Error converting IP address" << std::endl;
        return -1;
    }

    while (connect(sockd, (SOCKADDR*)&addr, sizeof(addr))!=0) {
        cerr << "Connection failed. Enter R to retry or enter E to exit: " << endl;
        getline(cin, input);
        transform(input.begin(), input.end(), input.begin(), ::tolower);
        while (input != "e" && input != "r") {
            cout << "Undefined command" << endl;
            cin >> input;
            transform(input.begin(), input.end(), input.begin(), ::tolower);
        }
        if (input == "e") {
            cout << "Terminating..." << endl;
            return 0;
        }
        else if (input == "r") {
            cout << "Retrying..." << endl;
        }
    }

    

    ZeroMemory(buf, PACKET_SIZE);
    if (recv(sockd, buf, PACKET_SIZE, 0) <= 0) {
        cout << "Error" << endl;
        return -1;
    }
    cout << buf << endl;//client number

    cout << "Connected!" << endl;
    while (1) {
        cout << "Enter login to log in or Enter signup to make account: " << endl;
        getline(cin, input);
        transform(input.begin(), input.end(), input.begin(), ::tolower);
        if (input == "login") {
            if (login(sockd) == -1) {
                cout << "Error occured" << endl;
                continue;
            }
            break;
        }
        else if (input == "signup") {
            if (signup(sockd) == -1) {
                cout << "Error occured" << endl;
            }
        }
        else if (input == "exit") {
            send(sockd, input.c_str(), sizeof(input), 0);
            closesocket(sockd);
            WSACleanup();
            return 0;
        }
        else {
            cout << "Undefined command!" << endl;
        }
    }
    cout << "Logged in!" << endl;
    if (recv(sockd, buf, PACKET_SIZE, 0) == -1) {
        return -1;
    }
    cout << buf << endl;

    thread(recv_from_server, ref(sockd)).detach();
    cout << "Enter your command: (Enter help for guide)" << endl;
    while (1) {
        getline(cin, input);
        transform(input.begin(), input.end(), input.begin(), ::tolower);
        
        string command = "";
        string target = "";
        string msg = "";
        for (int i = 0; i <input.size(); i++) {
            if (input[i] == ' ') {
                target_name_loc = i + 1;
                break;
            }
            else {
                command += input[i];
            }
        }
        if (command == "exit") {
            send(sockd, command.c_str(), sizeof(command), 0);
            closesocket(sockd);
            WSACleanup();
            return 0;
        }
        else if (command == "say") {
            send(sockd, input.c_str(), sizeof(input), 0);
        }
        else if (command == "whisper") {
            for (int i = target_name_loc; i < input.size(); i++) {
                if (input[i] == ' ') {
                    msg_loc = i + 1;
                    break;
                }
                else {
                    target += input[i];
                }
            }
            for (int i = msg_loc; i < input.size(); i++) {
                if (input[i] == ' ') {
                    msg_loc = i + 1;
                    break;
                }
                else {
                    msg += input[i];
                }
            }
            time(&timer);
            localtime_s(&t, &timer);
            cout << "[" + to_string(t.tm_year + 1900) + "-" + to_string(t.tm_mon + 1) + "-" + to_string(t.tm_mday) + " "
                + to_string(t.tm_hour) + ":" + to_string(t.tm_min) + "]" + "(Whisper)" + target + "<< " + msg << endl;
            send(sockd, input.c_str(), sizeof(input), 0);
        }
        else if (command == "users") {
            send(sockd, input.c_str(), sizeof(input), 0);
        }
        else if (command == "help") {
            cout<<helpmessage << endl;
        }
        else {
            cout << "Undefined command!" << endl;
        }
    }

}