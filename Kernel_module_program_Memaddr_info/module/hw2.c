#include <linux/module.h>
#include <linux/kernel.h>
#include <linux/interrupt.h>
#include <linux/timer.h>
#include <linux/mm.h>
#include <linux/proc_fs.h>
#include <linux/seq_file.h>
#include <linux/cpumask.h>
#include <linux/sched.h>
#include <linux/spinlock.h>
#include <linux/jiffies.h>
#define PROC_NAME "hw2"

MODULE_AUTHOR("BAE, YoungWoo");
MODULE_LICENSE("GPL");

static struct tasklet_struct tl;//tasklet
static struct timer_list tm;//timer for tasklet
DEFINE_SPINLOCK(lock);//lock for collecting info

struct code_area_start{//infos of start of code_area
	unsigned long v;//virtual address
	pgd_t *pgd;//pgd address
	pud_t *pud;//pud address
	pmd_t *pmd;//pmd address
	pte_t *pte;//pte address
	unsigned long p;//physical address
};

struct code_area_end{//infos of end of code_area
        unsigned long v;//virtual address
        pgd_t *pgd;//pgd address
        pud_t *pud;//pud address
        pmd_t *pmd;//pmd address
        pte_t *pte;//pte address
        unsigned long p;//physical address
};

struct data_area{//infos of data_area struct
        unsigned long vs;//virtual / start address
        unsigned long ps;//physical / start address
        unsigned long ve;//virtual / end address
	unsigned long pe;//physical / end address
};

struct heap_area{//infos of heap_area struct
        unsigned long vs;//virtual / start address
        unsigned long ps;//physical / start address
	unsigned long ve;//virtual / end address
	unsigned long pe;//virtual / end address
};
	
struct stack_area{//infos of stack_area struct
        unsigned long vs;//virtual / start address
        unsigned long ps;//physical / start address
        unsigned long ve;//virtual / end address
        unsigned long pe;//physical / end address
};



struct hw2_struct{
	char comm[32];//name of task
	pid_t pid;//process id of task
	int uptime;//uptime when the information gathered
	unsigned long long int stime;//start time of task
	unsigned long base_addr;//PGD BASE ADDRESS
	struct code_area_start cas;//infos of Code area start
	struct code_area_end cae;//infos of Code area end
	struct data_area da;//infos of Data area
	struct heap_area ha;//infos of Heap area
	struct stack_area sa;//infos of Stack area
};

static struct hw2_struct hw2_info[5];//5 latest infos

void gather(unsigned long data){
	struct task_struct *task;//task pointer
	struct mm_struct *mm;//mm
	struct task_struct lt;//latest task
	struct hw2_struct hw2;//info struct
	struct code_area_start cas;//code area start info
	struct code_area_end cae;//code area end info
	struct data_area da;//data area info
	struct heap_area ha;//heap area info
	struct stack_area sa;//stack area info
	int uptime;//uptime when collect infos
	p4d_t *p4d;//p4d address for getting pud address because of 5-level paging
	lt.start_time = 0;
	spin_lock(&lock);//lock
	for_each_process(task){//check every process task_struct
		if(lt.start_time < task->start_time && task->pid != 0 && task->mm != NULL){//if not kernel thread and start_time is bigger than existing one's
			lt = *task;//store task that checking now to lt
			mm = get_task_mm(task);//store mm_struct of lt to mm
		} 
	}
	uptime = jiffies_to_msecs(jiffies - INITIAL_JIFFIES)/1000;//(current jiffies - jiffies when boot)/HZ
	hw2.uptime = uptime;//store uptime to info struct
	hw2.base_addr = pgd_offset(mm,0);//store PGD base address using pgd_offset() with address = 0
	hw2.pid = lt.pid;//store pid to struct
	strncpy(hw2.comm,lt.comm,31);//store command name to struct
	hw2.comm[31] = '\0';//end of command name array is new line char
	hw2.stime = lt.start_time;//store start time to struct
	//code area start here
	cas.v = mm->start_code;//mm->start_code is virtual address of start of code area of task.
	cas.pgd = pgd_offset(mm,mm->start_code);//pgd address of mm using pgd_offset() with address = mm->start_code
	p4d = p4d_offset(cas.pgd,mm->start_code);//p4d address of mm using p4d_offset() with pgd = cas.pgd and address = mm->start_code
	cas.pud = pud_offset(p4d,mm->start_code);//pud address of mm
	cas.pmd = pmd_offset(cas.pud,mm->start_code);//pmd address of mm
	cas.pte = pte_offset_map(cas.pmd,mm->start_code);//pte address
	cas.p = pte_page(*(cas.pte));//physical address using pte_page and value of pte address
	hw2.cas = cas;//store code area start infos to info struct
	//code area end here
	cae.v = mm->end_code;//mm->end_code is virtual address of end of code area of task
	cae.pgd = pgd_offset(mm,mm->end_code);//same as cas
	cae.pud = pud_offset(p4d_offset(cas.pgd,mm->end_code),mm->end_code);//same as cas
	cae.pmd = pmd_offset(cas.pud,mm->end_code);//same as cas
	cae.pte = pte_offset_map(cas.pmd,mm->end_code);//same as cas
	cae.p = pte_page(*(cae.pte));//same as cas
	hw2.cae = cae;//store code area end infos to info struct
	//data area here
	da.vs = mm->start_data;//mm->start_data is virtual address of start of data area
	da.ps = da.vs - PAGE_OFFSET;//to convert virtual address to physical address, get lower address by PAGE_OFFSET
	da.ve = mm->end_data;//mm->end_data is virtual address of end of data area
	da.pe = da.ve - PAGE_OFFSET;//same as da.ps
	hw2.da = da;//store data area infos to info struct
	//heap area here
	ha.vs = mm->start_brk;//mm->start_brk is virtual address of start of heap area
	ha.ps = ha.vs - PAGE_OFFSET;//convert virtual addr to physical addr
	ha.ve = mm->brk;//mm->brk is virtual address of end of heap area
	ha.pe = ha.ve - PAGE_OFFSET;//convert virtual addr to physical addr
	hw2.ha = ha;//store heap area infos to info struct
	//stack area here
	sa.vs = mm->start_stack;//mm->start_stack is virtual address of start of stack area
	sa.ps = sa.vs - PAGE_OFFSET;//convert virtual addr to physical addr
	sa.ve = sa.vs + mm->stack_vm;//mm->stack_vm is size of stack area, so add the size to get virtual address of end of stack area
	sa.pe = sa.ve - PAGE_OFFSET;//convert virtual addr to physical addr
	hw2.sa = sa;// store stack area infos to info struct
	for(int i = 1;i<5;i++){
		hw2_info[i-1] = hw2_info[i];//oldest info expire and make space for new info. 0 is older, 4 is newer.
	}
	hw2_info[4] = hw2;//add new info struct to info struct array.
	spin_unlock(&lock);//unlock
}

static void tm_schedule(struct timer_list *t){
	tasklet_schedule(&tl);
	mod_timer(t,jiffies + msecs_to_jiffies(10000));
}



static void *hw2_seq_start(struct seq_file *s, loff_t *pos)
{
    static unsigned long counter = 0;
    if (*pos == 0)
        return &counter;
    else {
        *pos = 0;
        return NULL;
    }
}

static void *hw2_seq_next(struct seq_file *s, void *v, loff_t *pos)
{
    unsigned long *tmp_v = (unsigned long *)v;
    (*tmp_v)++;
    (*pos)++;
    return NULL;
}

static void hw2_seq_stop(struct seq_file *s, void *v)
{
    // nothing to do
}

static int hw2_seq_show(struct seq_file *s, void *v)
{
    loff_t *spos = (loff_t *) v;
    seq_printf(s, "[System Programming Assignment #2]\n");
    seq_printf(s, "ID: 2016147550\n");
    seq_printf(s, "Name: Bae, Youngwoo\n");
    seq_printf(s,"Uptime (s): %d\n",jiffies_to_msecs(jiffies - INITIAL_JIFFIES) / 1000);//current uptime
    seq_printf(s,"--------------------------------------------------\n");
    int tracenum = 0;//trace number variable because info struct array index doesn't match to i in for-loop
    for(int i = 0;i<5;i++){
	if(hw2_info[i].pid == 0){//if no information, do nothing.
		continue;
	}
    	seq_printf(s,"[Trace #%d]\n", tracenum++);//only if information exists, show trace and trace number increases
	//showing info struct members.
	seq_printf(s,"uptime(s): %d\n", hw2_info[i].uptime);
	seq_printf(s,"Command: %s\n",hw2_info[i].comm);
	seq_printf(s,"PID: %d\n",hw2_info[i].pid);
	seq_printf(s,"Start time (s): %lld\n",(hw2_info[i].stime/1000000000));
	seq_printf(s,"PGD base address: 0x%lx\n",hw2_info[i].base_addr);
	seq_printf(s,"Code Area\n");
	seq_printf(s,"- start (virtual): 0x%lx\n",hw2_info[i].cas.v);
	seq_printf(s,"- start (PGD): 0x%lx, 0x%lx\n",hw2_info[i].cas.pgd,pgd_val(*(hw2_info[i].cas.pgd)));//value of pgd using pgd_val
	seq_printf(s,"- start (PUD): 0x%lx, 0x%lx\n",hw2_info[i].cas.pud,pud_val(*(hw2_info[i].cas.pud)));//value of pud using pud_val
	seq_printf(s,"- start (PMD): 0x%lx, 0x%lx\n",hw2_info[i].cas.pmd,pmd_val(*(hw2_info[i].cas.pmd)));//value of pmd using pmd_val
	seq_printf(s,"- start (PTE): 0x%lx, 0x%lx\n",hw2_info[i].cas.pte,pte_val(*(hw2_info[i].cas.pte)));//value of pte using pte_val
	seq_printf(s,"- start (physical): 0x%lx\n",((pte_val(*hw2_info[i].cas.pte) & PAGE_MASK) | (hw2_info[i].cas.v & ~PAGE_MASK)));//bitwise opertaion to get physical address. pte value AND PAGE_MASK to get page offset, virtual address AND ~PAGE_MASK to get physical page offset, and page offset OR physical page offset to get physical address
	seq_printf(s,"- end (virtual): 0x%lx\n",hw2_info[i].cae.v);
	seq_printf(s,"- end (PGD): 0x%lx, 0x%lx\n",hw2_info[i].cae.pgd,pgd_val(*(hw2_info[i].cae.pgd)));
	seq_printf(s,"- end (PUD): 0x%lx, 0x%lx\n",hw2_info[i].cae.pud,pud_val(*(hw2_info[i].cae.pud)));
	seq_printf(s,"- end (PMD): 0x%lx, 0x%lx\n",hw2_info[i].cae.pmd,pmd_val(*(hw2_info[i].cae.pmd)));
	seq_printf(s,"- end (PTE): 0x%lx, 0x%lx\n",hw2_info[i].cae.pte,pte_val(*(hw2_info[i].cae.pte)));
	seq_printf(s,"- end (physical): 0x%lx\n",((pte_val(*hw2_info[i].cae.pte) & PAGE_MASK) | (hw2_info[i].cae.v & ~PAGE_MASK)));
	seq_printf(s,"Data Area\n");
	seq_printf(s,"- start (virtual): 0x%lx\n",hw2_info[i].da.vs);
	seq_printf(s,"- start (physical): 0x%lx\n",hw2_info[i].da.ps);
	seq_printf(s,"- end (virtual): 0x%lx\n",hw2_info[i].da.ve);
	seq_printf(s,"- end (physical): 0x%lx\n",hw2_info[i].da.pe);
	seq_printf(s,"Heap Area\n");
	seq_printf(s,"- start (virtual): 0x%lx\n",hw2_info[i].ha.vs);
        seq_printf(s,"- start (physical): 0x%lx\n",hw2_info[i].ha.ps);
        seq_printf(s,"- end (virtual): 0x%lx\n",hw2_info[i].ha.ve);
        seq_printf(s,"- end (physical): 0x%lx\n",hw2_info[i].ha.pe);
	seq_printf(s,"Stack Area\n");
        seq_printf(s,"- start (virtual): 0x%lx\n",hw2_info[i].sa.vs);
        seq_printf(s,"- start (physical): 0x%lx\n",hw2_info[i].sa.ps);
        seq_printf(s,"- end (virtual): 0x%lx\n",hw2_info[i].sa.ve);
        seq_printf(s,"- end (physical): 0x%lx\n",hw2_info[i].sa.pe);
    }

    return 0;
}

static struct seq_operations hw2_seq_ops = {
	.start = hw2_seq_start,
	.next = hw2_seq_next,
	.stop = hw2_seq_stop,
	.show = hw2_seq_show
};

static int hw2_proc_open(struct inode *inode, struct file *file){
	return seq_open(file, &hw2_seq_ops);
}

static const struct proc_ops hw2_proc_ops = {
	.proc_open = hw2_proc_open,
	.proc_read = seq_read,
	.proc_lseek = seq_lseek,
	.proc_release = seq_release
};

static int __init hw2_init(void){
	struct proc_dir_entry *proc_file_entry;
	proc_file_entry = proc_create(PROC_NAME,0,NULL, &hw2_proc_ops);

	timer_setup(&tm,tm_schedule,0);//when module start, timer start
	mod_timer(&tm,jiffies + msecs_to_jiffies(10000));//10000ms = 10s

	tasklet_init(&tl,gather,0);//every 10s, do gather(0)
	return 0;
}

static void __exit hw2_exit(void){
	tasklet_kill(&tl);
	del_timer_sync(&tm);
	remove_proc_entry(PROC_NAME, NULL);
}

module_init(hw2_init);
module_exit(hw2_exit);


