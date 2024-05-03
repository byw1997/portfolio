#ifndef _BETTER_LOCKED_HASH_TABLE_H_
#define _BETTER_LOCKED_HASH_TABLE_H_


#include <iostream>
#include <mutex>
#include <thread>
#include "hash_table.h"
#include "bucket.h"

#define LOCK_SIZE 50000

class better_locked_probing_hash_table : public hash_table {

  private:
    Bucket* table;
    const int TABLE_SIZE; //we do not consider resizing. Thus the table has to be larger than the max num items.

    /* TODO: put your own code here  (if you need something)*/
    /****************/
        std::mutex locks[LOCK_SIZE];

        uint32_t hash2(uint32_t x){
                x = ((x>>16) ^x)* 0x7feb352d;
                x = ((x>>15)^x)*0x846ca68d;
                x = (x>>16)^x;
                return (x % TABLE_SIZE);
        }

        uint32_t hash3(uint32_t x){
                x = ((x>>17)^x)*0xed5ad4bb;
                x = ((x>>11)^x)*0xac4c1b51;
                x = ((x>>15)^x)*0x31848bab;
                x = (x>>14)^x;
                return x;
        }

    /****************/
    /* TODO: put your own code here */

    public:

    better_locked_probing_hash_table(int table_size):TABLE_SIZE(table_size){
      this->table = new Bucket[TABLE_SIZE]();
      for(int i=0;i<TABLE_SIZE;i++) {
        this->table[i].valid=0; //means empty
      }
    }

    virtual uint32_t hash(uint32_t x)
    {
      //https://stackoverflow.com/questions/664014/what-integer-hash-function-are-good-that-accepts-an-integer-hash-key
      x = ((x >> 16) ^ x) * 0x45d9f3b;
      x = ((x >> 16) ^ x) * 0x45d9f3b;
      x = (x >> 16) ^ x;
      return (x % TABLE_SIZE);
    }

    virtual uint32_t hash_next(uint32_t key, uint32_t prev_index)
    {
      //linear probing. no special secondary hashfunction
      return ((prev_index + 1)% TABLE_SIZE);
    }

    //the buffer has to be allocated by the caller
    bool read(uint32_t key, uint64_t* value_buffer){
      /* TODO: put your own read function here */
      /****************/
            uint64_t index = this->hash(key);
            int probe_count = 0;
            uint64_t step = this->hash2(key);

            uint64_t original = index;

            while(table[index].valid == true){
                if(table[index].key == key){
                        *value_buffer = table[index].value;
                        return true;
                }
                else{
                        probe_count++;
                        index = (index + step) % TABLE_SIZE;
                        if(probe_count >= TABLE_SIZE) break;
                        if(index == original){
                                index = (index + 1)%TABLE_SIZE;
                                original = (original+1)%TABLE_SIZE;
                        }
                }
            }

            return false;




      /****************/
      /* TODO: put your own read function here */
    }


    bool insert(uint32_t key, uint64_t value) {
      /* TODO: put your own insert function here */
      /****************/
        uint64_t index = this->hash(key);
        int probe_count = 0;
        uint64_t step = this->hash2(key);

        uint64_t original = index;

        locks[index%LOCK_SIZE].lock();
         while(table[index].valid == true) {
                         if(table[index].key == key) {
                                 break;
                        } else {
                                locks[index%LOCK_SIZE].unlock();
                                probe_count++;
                                index = (index + step) % TABLE_SIZE;
                                if(index == original){
                                        index = (index+1) % TABLE_SIZE;
                                        original = (original+1) % TABLE_SIZE;
                                }
                                locks[index%LOCK_SIZE].lock();
                                if(probe_count >= TABLE_SIZE) return false;                             }
         }
        table[index].valid = true;
        table[index].key   = key;
        table[index].value = value;
        locks[index%LOCK_SIZE].unlock();
        return true;

      /****************/
      /* TODO: put your own insert function here */
    }

    int num_items() {
      int count=0;
      for(int i=0;i<TABLE_SIZE;i++) {
        if(table[i].valid==true) count++;
      }
      return count;
    }
};

#endif
