import psycopg
from psycopg import sql
import os
from typing import Union


# problem 1
def entire_search(CONNECTION: str, table_name: str) -> list:

    pass


# problem 2
def registration_history(CONNECTION: str, student_id: str) -> Union[list, None]:

    pass


# problem 3
def registration(CONNECTION: str, course_id: int, student_id: str) -> Union[list, None]:

    pass


# problem 4
def withdrawal_registration(CONNECTION: str, course_id: int, student_id: str) -> Union[list, None]:

    pass


# problem 5
def modify_lectureroom(CONNECTION: str, course_id: int, buildno: str, roomno: str) -> Union[list, None]:
    
    pass


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
