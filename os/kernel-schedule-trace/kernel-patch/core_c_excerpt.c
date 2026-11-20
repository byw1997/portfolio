/* Excerpt from kernel/sched/core.c (Linux, Ubuntu 22.04 kernel source).
 * Only the parts added for this assignment (between //hw1 markers) and
 * a few surrounding lines are kept. The rest of core.c is the unmodified kernel source. */

/* ---- near line 6490 ---- */
 * WARNING: must be called with preemption disabled!
 */
//hw1
struct hw1_sched_info HW1[20];
EXPORT_SYMBOL(HW1);
//hw1
static void __sched notrace __schedule(unsigned int sched_mode)
{
	struct task_struct *prev, *next;
	/* ... */

/* ---- inside __schedule(), after pick_next_task() ---- */
		switch_count = &prev->nvcsw;
	}

	next = pick_next_task(rq, prev, &rf);
	//hw1
	for(int i= 19;i > 0; i--){
		HW1[i] = HW1[i-1];
	}
	HW1[0].cpu_id = cpu;
	HW1[0].prev = prev;
	HW1[0].next = next;
	//hw1
	clear_tsk_need_resched(prev);
	clear_preempt_need_resched();
