#include <linux/module.h>
#include <linux/proc_fs.h>
#include <linux/seq_file.h>
#include <linux/cpumask.h>
#include <linux/sched.h>
#define PROC_NAME "hw1"

MODULE_AUTHOR("BAE, YoungWoo");
MODULE_LICENSE("GPL");

extern struct hw1_sched_info HW1[20];
static void *hw1_seq_start(struct seq_file *s, loff_t *pos)
{
    static unsigned long counter = 0;
    if (*pos == 0)
        return &counter;
    else {
        *pos = 0;
        return NULL;
    }
}

static void *hw1_seq_next(struct seq_file *s, void *v, loff_t *pos)
{
    unsigned long *tmp_v = (unsigned long *)v;
    (*tmp_v)++;
    (*pos)++;
    return NULL;
}

static void hw1_seq_stop(struct seq_file *s, void *v)
{
    // nothing to do
}

static int hw1_seq_show(struct seq_file *s, void *v)
{
    loff_t *spos = (loff_t *) v;
    seq_printf(s, "[System Programming Assignment #1]\n");
    seq_printf(s, "ID: 2016147550\n");
    seq_printf(s, "Name: Bae, Youngwoo\n");
    seq_printf(s,"# CPU: %d\n", num_online_cpus());
    seq_printf(s,"--------------------------------------------------\n");
    int tempindex;
    int ppolicy;
    int npolicy;
    extern struct hw1_sched_info HW1[20];
    struct hw1_sched_info tempinfo;
    for(int i = 0;i <20;i++){
	tempindex = 19 - i;
	tempinfo = HW1[tempindex];
	struct task_struct *prev = tempinfo.prev;
	struct task_struct *next = tempinfo.next;
	ppolicy = prev->policy;
	npolicy = next->policy;
    	seq_printf(s,"schedule() trace #%d -CPU #%d\n",i,tempinfo.cpu_id);
	seq_printf(s,"Command: %s\n", prev->comm);
	seq_printf(s,"PID: %d\n",prev->pid);
	seq_printf(s,"Priority: %d\n",prev->prio);
	seq_printf(s,"Start time (ms):%lld\n",(prev->start_time/1000000));
	switch(ppolicy) {
		case 0:
			seq_printf(s,"Scheduler: CFS\n");
			break;
		case 1:
			seq_printf(s,"Scheduler: RT\n");
			break;
		case 2:
			seq_printf(s,"Scheduler: RT\n");
                        break;
		case 3:
			seq_printf(s,"Scheduler: CFS\n");
                        break;
		case 5:
			seq_printf(s,"Scheduler: IDLE\n");
			break;
		case 6:
			seq_printf(s,"Scheduler: RD\n");
			break;
	}
	seq_printf(s,"->\n");
	seq_printf(s,"Command: %s\n",next->comm);
        seq_printf(s,"PID: %d\n",next->pid);
        seq_printf(s,"Priority: %d\n",next->prio);
        seq_printf(s,"Start time (ms):%lld\n",(next->start_time/1000000));
	switch(npolicy) {
                case 0:
                        seq_printf(s,"Scheduler: CFS\n");
                        break;
                case 1:
                        seq_printf(s,"Scheduler: RT\n");
                        break;
                case 2:
                        seq_printf(s,"Scheduler: RT\n");
                        break;
		case 3:
			seq_printf(s,"Scheduler: CFS\n");
                        break;
                case 5:
                        seq_printf(s,"Scheduler: IDLE\n");
                        break;
                case 6:
                        seq_printf(s,"Scheduler: RD\n");
                        break;
        }
	seq_printf(s,"--------------------------------------------------\n");
    }
    return 0;
}

static struct seq_operations hw1_seq_ops = {
	.start = hw1_seq_start,
	.next = hw1_seq_next,
	.stop = hw1_seq_stop,
	.show = hw1_seq_show
};

static int hw1_proc_open(struct inode *inode, struct file *file){
	return seq_open(file, &hw1_seq_ops);
}

static const struct proc_ops hw1_proc_ops = {
	.proc_open = hw1_proc_open,
	.proc_read = seq_read,
	.proc_lseek = seq_lseek,
	.proc_release = seq_release
};

static int __init hw1_init(void){
	struct proc_dir_entry *proc_file_entry;
	proc_file_entry = proc_create(PROC_NAME,0,NULL, &hw1_proc_ops);
	return 0;
}

static void __exit hw1_exit(void){
	remove_proc_entry(PROC_NAME, NULL);
}

module_init(hw1_init);
module_exit(hw1_exit);
