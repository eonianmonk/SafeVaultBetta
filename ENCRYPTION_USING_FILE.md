

------Password hash:------

row = Read 32 bytes starting from file.length/2 
hash = sha256(row) // password

------Decryption key:----- (Encryption key)

row = Read 32 bytes starting from file.length/2 - 32
key = sha256(row)