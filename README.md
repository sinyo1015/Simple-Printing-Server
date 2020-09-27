# Simple Printing Server

Dunno man, ini cuma simple server c# printing server. Biasanya dipakai aplikasi POS (Point Of Sales),

Contoh `config.json` :
```json
{
    "NamaToko" : "Nama toko disini",
    "AlamatToko" : "Alamat toko disini",
    "DefaultPrinter" : "Printer yang akan digunakan untuk printing biasanya"
}
```
.
Aplikasi ini berjalan di `http://localhost:7001`. Apabila port `:7001` menerima POST Request maka server akan meneruskan request ke printer dengan metode Bypass Printer

---Requirement---
- Microsoft Visual Studio 2017
- Net Framework 4.6.1


---Library Used---
- [Newtonsoft JSON](https://github.com/JamesNK/Newtonsoft.Json)
- [Rawprinter](https://github.com/frogmorecs/RawPrint)
- [PrinterUtility (c)eduardo.cecon ](https://www.nuget.org/packages/PrinterUtility/)
