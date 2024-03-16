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
	{ 0x46e85567, "HW1" },
	{ 0xc6bc5007, "seq_printf" },
	{ 0xc60d0620, "__num_online_cpus" },
	{ 0x87a21cb3, "__ubsan_handle_out_of_bounds" },
	{ 0x513b0374, "remove_proc_entry" },
	{ 0x8523cf72, "seq_read" },
	{ 0x53dfc459, "seq_lseek" },
	{ 0x4d4ff4a9, "seq_release" },
	{ 0xbdfb6dbb, "__fentry__" },
	{ 0x5b8239ca, "__x86_return_thunk" },
	{ 0x37b9fa9b, "proc_create" },
	{ 0x77f04ce5, "seq_open" },
	{ 0xe055095d, "module_layout" },
};

MODULE_INFO(depends, "");


MODULE_INFO(srcversion, "A0D62EFB253474DDB319FB8");
