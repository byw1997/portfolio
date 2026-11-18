/* Excerpt from include/linux/sched.h. Lines between //hw1 markers were added. */

//hw1
struct hw1_sched_info{
	int cpu_id;
	struct task_struct *prev;
	struct task_struct *next;
};

extern struct hw1_sched_info HW1[20];

//hw1
