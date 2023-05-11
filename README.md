# Trivium cipher

Trivium is a synchronous stream cipher designed to provide a flexible trade-off between speed and gate count in hardware, and reasonably efficient software implementation.

Trivium was submitted to the Profile II (hardware) of the eSTREAM competition by its authors, Christophe De Cannière and Bart Preneel, and has been selected as part of the portfolio for low area hardware ciphers (Profile 2) by the eSTREAM project. It is not patented and has been specified as an International Standard under ISO/IEC 29192-3.[1]

It generates up to 264 bits of output from an 80-bit key and an 80-bit IV. It is the simplest eSTREAM entrant; while it shows remarkable resistance to cryptanalysis for its simplicity and performance, recent attacks leave the security margin looking rather slim

# Usage

Provide 80-bit key and IV, then create new instance of Trivium class and initalize state

```
  byte[] key = new byte[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
  byte[] iv = new byte[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

  Trivium triv = new Trivium(key, iv);
  triv.Init();
```

Encryption

```
byte[] encryptedBytes = triv.Crypt("Hello World!");
```

Decryption
```
string result = decrypt.Crypt(encryptedBytes);
```

# References
https://www.ecrypt.eu.org/stream/p3ciphers/trivium/trivium_p3.pdf
