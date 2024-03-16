#include <linux/module.h>
#define INCLUDE_VERMAGIC
#include <linux/build-salt.h>
#include <linux/elfnote-lto.h>
#include <linux/export-internal.h>
#include <linux/vermagic.h>
#include <linux/compiler.h>

BUILD_SALT;
BUILD_LTO_INFO;

MODULE_INFO(vermagic, VERMAGIC_STRING);
MODULE_INFO(name, KBUILD_MODNAME);

__visible struct module __this_module
__section(".gnu.linkonce.this_module") = {
	.name = KBUILD_MODNAME,
	.init = init_module,
#ifdef CONFIG_MODULE_UNLOAD
	.exit = cleanup_module,
#endif
	.arch = MODULE_ARCH_INIT,
};

#ifdef CONFIG_RETPOLINE
MODULE_INFO(retpoline, "Y");
#endif


static const struct modversion_info ____versions[]
__used __section("__versions") = {
	{ 0x2364c85a, "tasklet_init" },
	{ 0xb12fd881, "seq_open" },
	{ 0xea3c74e, "tasklet_kill" },
	{ 0x82ee90dc, "timer_delete_sync" },
	{ 0xac0dde12, "remove_proc_entry" },
	{ 0x9d2ab8ac, "__tasklet_schedule" },
	{ 0xd65b4e21, "seq_printf" },
	{ 0x37befc70, "jiffies_to_msecs" },
	{ 0xbec1f61d, "pv_ops" },
	{ 0x87a21cb3, "__ubsan_handle_out_of_bounds" },
	{ 0xba8fbd64, "_raw_spin_lock" },
	{ 0x854ad34a, "init_task" },
	{ 0x69acdf38, "memcpy" },
	{ 0xea7c0f9b, "get_task_mm" },
	{ 0x72d79d83, "pgdir_shift" },
	{ 0x9166fada, "strncpy" },
	{ 0xd1d6e0b7, "boot_cpu_data" },
	{ 0xdad13544, "ptrs_per_p4d" },
	{ 0x1d19f77b, "physical_mask" },
	{ 0x7cd8d75e, "page_offset_base" },
	{ 0x97651e6c, "vmemmap_base" },
	{ 0xb5b54b34, "_raw_spin_unlock" },
	{ 0xa19b956, "__stack_chk_fail" },
	{ 0xa648e561, "__ubsan_handle_shift_out_of_bounds" },
	{ 0xd8e57183, "seq_read" },
	{ 0x6e7fd3b8, "seq_lseek" },
	{ 0x8c5a766b, "seq_release" },
	{ 0xbdfb6dbb, "__fentry__" },
	{ 0x5b8239ca, "__x86_return_thunk" },
	{ 0x45db931b, "proc_create" },
	{ 0xc6f46339, "init_timer_key" },
	{ 0x15ba50a6, "jiffies" },
	{ 0xc38c83b8, "mod_timer" },
	{ 0x453e7dc, "module_layout" },
};

MODULE_INFO(depends, "");


MODULE_INFO(srcversion, "13EE30EDFD9E17DD8E5B70B");
