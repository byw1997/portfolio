use sha2::Sha256;
use hmac::{Hmac,Mac};
use openssl::symm::{encrypt,decrypt, Cipher};
use std::fs::File;
use std::io::{Read,Write};

fn errorexit(){
    println!("ERROR");
    std::process::exit(2);
}

fn findtag(arg:Vec<String>) -> String{
    for i in 2..arg.len()-1{
        if arg[i] == "-tag"{
            if i+1 >= arg.len(){
                errorexit();
            }
            else if arg[i+1].chars().nth(0).unwrap() == '-'{
                errorexit();
            }
            return arg[i+1].clone();
        }
    }
    errorexit();
    return "".to_string();
}

fn findout(arg:Vec<String>) -> String{
    for i in 2..arg.len()-1{
        if arg[i] == "-out"{
            if i+1 >= arg.len(){
                errorexit();
            }
            else if arg[i+1].chars().nth(0).unwrap() == '-'{
                errorexit();
            }
            return arg[i+1].clone();
        }
    }
    errorexit();
    return "".to_string();
}


fn findin(arg:Vec<String>) -> String{
    for i in 2..arg.len()-1{
        if arg[i] == "-in"{
            if i+1 >= arg.len(){
                errorexit();
            }
            else if arg[i+1].chars().nth(0).unwrap() == '-'{
                errorexit();
            }
            return arg[i+1].clone();
        }
    }
    errorexit();
    return "".to_string();
}

fn findkey(arg:Vec<String>) -> String{
    for i in 2..arg.len()-1 {
        if arg[i] == "-key"{
            if i+1 >= arg.len(){
                errorexit();
            }
            else if arg[i+1].chars().nth(0).unwrap() == '-'{
                errorexit();
            }
            return arg[i+1].clone();
        }
    }
    errorexit();
    return "".to_string();
}

fn inputparser(arg:Vec<String>) -> Vec<String>{
    let mut ret = Vec::new();
    let key:String = findkey(arg.clone());
    let input:String = findin(arg.clone());
    let output:String = findout(arg.clone());
    let tag:String = findtag(arg.clone());
    ret.push(key);
    ret.push(input);
    ret.push(output);
    ret.push(tag);
    return ret;
}

fn getdata(filename:String) -> String{
    let mut data = String::new();
    let _ = File::open(filename).unwrap().read_to_string(&mut data);
    return data;
}

fn gethexdata(filename:String) -> Vec<u8>{
    let mut data = Vec::new();
    let _ = File::open(filename).unwrap().read_to_end(&mut data);
    return data;
}

fn enc(opts:Vec<String>){
    let cipher = Cipher::aes_256_cbc();
    let key = getdata(opts[0].clone());
    let mut key = key.as_bytes().to_vec();

    for _ in key.len()..32{
        key.push(0);
    }
    let key = &key[0..32];
    let input = getdata(opts[1].clone());
    let input = input.as_bytes();
    let iv = b"\x00\x01\x02\x03\x04\x05\x06\x07\x00\x01\x02\x03\x04\x05\x06\x07";
    let ciphertext = encrypt(cipher,key,Some(iv),input).unwrap();
    let outputfile = &opts[2];
    let mut file = File::create(outputfile).unwrap();
    file.write_all(&ciphertext).unwrap();
    let mut mac = Hmac::<Sha256>::new_from_slice(key).unwrap();
    mac.update(&ciphertext);
    let result = mac.finalize();
    let code_bytes = result.into_bytes();
    let tagfile = &opts[3];
    let mut tag = File::create(tagfile).unwrap();
    tag.write_all(&code_bytes).unwrap();
}

fn verifyerror(){
    println!("VERIFICATION FAILURE");
    std::process::exit(1);
}

fn dec(opts:Vec<String>){
    let cipher = Cipher::aes_256_cbc();
    let key = getdata(opts[0].clone());
    let mut key = key.as_bytes().to_vec();

    for _ in key.len()..32{
        key.push(0);
    }
    let key = &key[0..32];
    let input = gethexdata(opts[1].clone());
    let tag = gethexdata(opts[3].clone());
    let mut mac = Hmac::<Sha256>::new_from_slice(key).unwrap();
    mac.update(&input);
    match mac.verify_slice(&tag[..]){
        Ok(())=>{}
        Err(_mac_error)=>{
            verifyerror();
        }
    }
    let iv = b"\x00\x01\x02\x03\x04\x05\x06\x07\x00\x01\x02\x03\x04\x05\x06\x07";
    let decrypted = decrypt(cipher,key,Some(iv),&input).unwrap();
    let outputfile = &opts[2];
    let mut out = File::create(outputfile).unwrap();
    out.write_all(&decrypted).unwrap();

}

fn main(){
    let args: Vec<String> = std::env::args().collect();
    if args.len() != 10 {
        errorexit();
    }
    let command = &args[1];
    let options = inputparser(args.clone());
    if command == "enc"{
        enc(options.clone());
    }
    else if command == "dec"{
        dec(options.clone());
    }
    else{
        errorexit();
    }
    std::process::exit(0);
}
