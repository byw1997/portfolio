use std::env;
use std::str;
use std::fs::File;
use std::io::{self,Read,BufRead, BufReader,Write};
use std::sync::Arc;
use std::path::{Path,PathBuf};
use darknet::image_classifier;
use nix::sched::{unshare,clone,CloneFlags};
use nix::sys::signal::Signal;
use nix::sys::wait::waitpid;
use nix::mount::{mount,umount,MsFlags};
use nix::unistd::{pipe,close,dup2,Uid,Gid,getuid,getgid,setegid,seteuid,sethostname};
use std::os::fd::{RawFd,FromRawFd,IntoRawFd,AsRawFd};
use std::process::{Command,exit};
use nix::libc;


fn classify(user:String, filename:String,top_k:i32) -> isize{
    println!("[{}:{}:{}]",user.clone(),filename.clone(),top_k);
    let path = format!("data/{}",user.clone());
    let source = PathBuf::from(path.clone());
    let flags = MsFlags::MS_BIND|MsFlags::MS_PRIVATE|MsFlags::MS_MOVE|MsFlags::MS_RDONLY|MsFlags::MS_REC|MsFlags::MS_NOEXEC;
    let _ = std::fs::copy("darknet.weights",format!("data/{}/darknet.weights",user)).unwrap();
    let _ = std::fs::copy("imagenet.shortnames.list",format!("data/{}/imagenet.shortnames.list",user)).unwrap();
    let _ = std::fs::copy("darknet.cfg",format!("data/{}/darknet.cfg",user)).unwrap();

    std::fs::create_dir_all(format!("data/{}/usr/local/lib",user)).unwrap();
    let _ = std::fs::copy("darknet.cfg",format!("data/{}/usr/local/lib/darknet.cfg",user)).unwrap();
    let _ = std::fs::copy("libdarknet_predict.so",format!("data/{}/usr/local/lib/libdarknet_predict.so",user)).unwrap();



    let wd = env::current_dir().unwrap();
    env::set_current_dir(format!("data/{}",user)).unwrap();
    let _ = unshare(CloneFlags::CLONE_NEWNS | CloneFlags::CLONE_NEWUTS | CloneFlags::CLONE_NEWIPC | CloneFlags::CLONE_NEWNET);
    sethostname(&user).unwrap();
    let _ = mount(Some(&wd),"/",None::<&str>,flags,None::<&str>).unwrap();
    let cstr = std::ffi::CString::new(filename).unwrap();
    unsafe{
        let result = image_classifier(cstr.as_ptr(),top_k);
    }
    let _ = umount("/");
    let _ = std::fs::remove_file(format!("data/{}/darknet.weights",user));
    let _ = std::fs::remove_file(format!("data/{}/imagenet.shortnames.list",user));
    let _ = std::fs::remove_dir_all(format!("data/{}/usr",user));
    exit(0);
}

fn get_request() -> (Vec<String>,Vec<String>,Vec<i32>){
    let mut user:Vec<String> = Vec::new();
    let mut filename:Vec<String> = Vec::new();
    let mut top_k:Vec<i32> = Vec::new();
    //let mut request = Vec::new();
    let request = File::open("requests.txt").unwrap();
    let reader = BufReader::new(request);
    for line in reader.lines(){
        let line = line.unwrap();
        let temp:Vec<&str> = line.split(':').collect();
        //println!("user: {}, filename: {},top_k: {}.",temp[0],temp[1],temp[2]);
        let _ = user.push(temp[0].to_string());
        let _ = filename.push(temp[1].to_string());
        let _ = top_k.push(temp[2].parse::<i32>().unwrap());
    }
    return (user, filename, top_k);
}

fn main() {
    let mut out = File::create("results.txt").unwrap();
    let (user, filename,top_k) = get_request();
    let Cflags = CloneFlags::CLONE_NEWNS | CloneFlags::CLONE_NEWUTS | CloneFlags::CLONE_NEWIPC | CloneFlags::CLONE_NEWNET;
    const STACK_SIZE:usize = 1024*1024;
    let mut stack = vec![0;STACK_SIZE];
    seteuid(Uid::from_raw(0));
    setegid(Gid::from_raw(0));
    let out_fd = out.as_raw_fd();
    unsafe{libc::dup2(out_fd,libc::STDOUT_FILENO);}
    for i in 0..user.len(){
        let pid = unsafe{clone(Box::new(||classify(user[i].clone(),filename[i].clone(),top_k[i])), stack.as_mut_slice(), Cflags, Some(Signal::SIGCHLD as i32)).unwrap()};
        waitpid(pid,None);
    }
}

