import psycopg
from psycopg import sql
import os
from typing import Union


# problem 1
def entire_search(CONNECTION: str, table_name: str) -> list:
    with psycopg.connect(CONNECTION) as conn:
        with conn.cursor() as cur:
            cur.execute(sql.SQL("SELECT * FROM {table}").format(table = sql.Identifier("myschema",table_name)))
            records = cur.fetchall()
            for record in records:
                print(record)
    return records

# problem 2
def registration_history(CONNECTION: str, student_id: str) -> Union[list, None]:
    with psycopg.connect(CONNECTION) as conn:
        with conn.cursor() as cur:
            query = sql.SQL("""SELECT C.{YEAR}, C.{SEMESTER}, C.{COURSE_ID_PREFIX}, C.{COURSE_ID_NO}, C.{DIVISION_NO}, C.{COURSE_NAME}, F.{NAME}, G.{GRADEPOINT}
            FROM {COURSE_REG} AS R, {COURSE} AS C, {FACULTY} AS F, {GRADE} as G
            WHERE (R.{STUDENT_ID}, R.{STUDENT_ID}, R.{COURSE_ID}, C.{PROF_ID}, G.{COURSE_ID}) = (%s, G.{STUDENT_ID}, C.{COURSE_ID}, F.{ID},R.{COURSE_ID})
            ORDER BY C.{YEAR} ASC, C.{SEMESTER} ASC, C.{COURSE_NAME} ASC
            ;""").format(COURSE_REG = sql.Identifier("myschema", "course_registration"),
                        COURSE = sql.Identifier("myschema", "course"),
                        FACULTY = sql.Identifier("myschema", "faculty"),
                        GRADE = sql.Identifier("myschema", "grade"),
                        YEAR = sql.Identifier("YEAR"),
                        SEMESTER = sql.Identifier("SEMESTER"),
                        COURSE_ID_PREFIX = sql.Identifier("COURSE_ID_PREFIX"),
                        COURSE_ID_NO = sql.Identifier("COURSE_ID_NO"),
                        DIVISION_NO = sql.Identifier("DIVISION_NO"),
                        NAME = sql.Identifier("NAME"),
                        COURSE_NAME = sql.Identifier("COURSE_NAME"),
                        ID = sql.Identifier("ID"),
                        GRADEPOINT = sql.Identifier("GRADE"),
                        STUDENT_ID = sql.Identifier("STUDENT_ID"),
                        COURSE_ID = sql.Identifier("COURSE_ID"),
                        PROF_ID = sql.Identifier("PROF_ID")
            )
            cur.execute(query,(student_id,))
            records = cur.fetchall()
            if records:
                for record in records:
                    print(record)
            else:
                print("Not Exist student with STUDENT ID:",student_id)
    return records


# problem 3
def registration(CONNECTION: str, course_id: int, student_id: str) -> Union[list, None]:
    with psycopg.connect(CONNECTION) as conn:
        with conn.cursor() as cur:
            query = sql.SQL("INSERT INTO {} VALUES (%s,%s)").format(sql.Identifier("myschema","course_registration"))
            try:
                cur.execute(query, (course_id,student_id))
                
                record = cur.fetchone()
                conn.commit()
                print(record)
                return record
            except psycopg.Error as err:
                errcode = err.diag.sqlstate
                if errcode == '23503':
                    errmsg = err.diag.constraint_name
                    if errmsg == 'CO_REG_1':
                        print("Not Exist course with COURSE ID:",course_id)
                    elif errmsg == 'STU_REG_1':
                        print("Not Exist student with STUDENT ID:",student_id)
                elif errcode == '23505':
                    conn.rollback()
                    q = sql.SQL("""SELECT S.{SNAME},C.{CNAME}
                                FROM {ST} AS S, {CR} AS C
                                WHERE S.{SID} = %s and C.{CID} = %s;""").format(
                        ST = sql.Identifier("myschema", "students"),
                        CR = sql.Identifier("myschema", "course"),
                        SNAME = sql.Identifier("NAME"),
                        CNAME = sql.Identifier("COURSE_NAME"),
                        SID = sql.Identifier("STUDENT_ID"),
                        CID = sql.Identifier("COURSE_ID"))
                    cur.execute(q,(student_id, course_id))
                    r = cur.fetchone()
                    print(r[0],"is already registrated in", r[1])

# problem 4
def withdrawal_registration(CONNECTION: str, course_id: int, student_id: str) -> Union[list, None]:
    with psycopg.connect(CONNECTION) as conn:
        with conn.cursor() as cur:
            query = sql.SQL("""DELETE FROM {REG} as r WHERE (r.{CID},r.{SID})= (%s,%s)""").format(
                REG = sql.Identifier("myschema","course_registration"),
                SID = sql.Identifier("STUDENT_ID"),
                CID = sql.Identifier("COURSE_ID"))
            cur.execute(query,(course_id,student_id))
            records = cur.statusmessage.split()
            conn.commit()
            if records[1] == '0':
                q1 = sql.SQL("""SELECT S.{SNAME}
                            FROM {ST} AS S
                            WHERE S.{SID} = %s;""").format(
                    ST = sql.Identifier("myschema", "students"),                
                    SNAME = sql.Identifier("NAME"),       
                    SID = sql.Identifier("STUDENT_ID"))   
                q2 = sql.SQL("""SELECT C.{CNAME}
                            FROM {CR} AS C
                            WHERE C.{CID} = %s;""").format(
                    CR = sql.Identifier("myschema", "course"),
                    CNAME = sql.Identifier("COURSE_NAME"),
                    CID = sql.Identifier("COURSE_ID"))
                cur.execute(q2,(course_id,))
                r = cur.fetchone()             
                if r:
                    cname = r[0]
                    cur.execute(q1,(student_id,))
                    r = cur.fetchone()
                    if r:
                        sname = r[0]
                        print(sname, "is not registrated in",cname)
                    else:
                        print("Not Exist student with STUDENT ID:", student_id)
                else:
                    print("Not Exist course with COURSE ID:", course_id)
                    


# problem 5
def modify_lectureroom(CONNECTION: str, course_id: int, buildno: str, roomno: str) -> Union[list, None]:
    with psycopg.connect(CONNECTION) as conn:
        with conn.cursor() as cur:
            q1 = sql.SQL("""SELECT C.{CNAME}
                            FROM {CR} AS C
                            WHERE C.{CID} = %s;""").format(
                    CR = sql.Identifier("myschema", "course"),
                    CNAME = sql.Identifier("COURSE_NAME"),
                    CID = sql.Identifier("COURSE_ID"))
            cur.execute(q1,(course_id,))
            r=cur.fetchone()
            if r:
                q2 = sql.SQL("""SELECT L.{BNO}, L.{RNO}
                            FROM {LR} AS L
                            WHERE L.{BNO} = %s and L.{RNO} = %s;""").format(
                    LR = sql.Identifier("myschema", "lectureroom"),
                    BNO = sql.Identifier("BUILDNO"),
                    RNO = sql.Identifier("ROOMNO"))
                cur.execute(q2,(buildno, roomno))
                r=cur.fetchone()
                if r:
                    q3 = sql.SQL("""UPDATE {COURSE} AS C
                    SET {BNO} = %s,
                    {RNO} = %s
                    WHERE {CID} = %s
                    """).format(
                    COURSE = sql.Identifier("myschema","course"),
                    CID = sql.Identifier("COURSE_ID"),
                    BNO = sql.Identifier("BUILDNO"),
                    RNO = sql.Identifier("ROOMNO"))
                    cur.execute(q3,(buildno,roomno,course_id))
                    conn.commit()
                else:
                    print("Not Exist lecture room with BUILD NO:",buildno, "/ ROOM NO:", roomno)
            else:
                print("Not Exist course with COURSE ID:", course_id)


# sql file execute ( Not Edit )
def execute_sql(CONNECTION, path):
    folder_path = '/'.join(path.split('/')[:-1])
    file = path.split('/')[-1]
    if file in os.listdir(folder_path):
        with psycopg.connect(CONNECTION) as conn:
            conn.execute(open(path, 'r', encoding='utf-8').read())
            conn.commit()
        print("{} EXECUTRED!".format(file))
    else:
        print("{} File Not Exist in {}".format(file, folder_path))