using System;

namespace PrinterServer
{
    public enum JenisKuantitas
    {
        Box = 1,
        Lusin = 2,
        Pcs = 3
    }

    public class BarangBelanjaan
    {
        public string NamaBarang { get; set; }
        public JenisKuantitas JenisKuantitas { get; set; }
        public int JumlahBeli { get; set; }
        public UInt64 HargaProduk { get; set; }
        public UInt64 HargaJumlahProduk { get; set; }
        public bool IsGrosir { get; set; }
    }
}
