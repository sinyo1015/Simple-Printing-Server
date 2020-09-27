
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PrinterUtility;
using RawPrint;
using PrinterUtility.EscPosEpsonCommands;

namespace PrinterServer
{
    public class RawPrinterHelper
    {

        private EscPosEpson esCmd = new EscPosEpson();
        private byte[] holder = Encoding.ASCII.GetBytes(" ");

        public RawPrinterHelper CetakHeader(string namaToko, string alamat)
        {
            holder = PrintExtensions.AddBytes(holder, esCmd.CharSize.DoubleWidth6());
            holder = PrintExtensions.AddBytes(holder, esCmd.FontSelect.FontA());
            holder = PrintExtensions.AddBytes(holder, esCmd.Alignment.Center());
            holder = PrintExtensions.AddBytes(holder, Encoding.ASCII.GetBytes(namaToko + "\n"));
            holder = PrintExtensions.AddBytes(holder, esCmd.Alignment.Center());
            holder = PrintExtensions.AddBytes(holder, esCmd.CharSize.Nomarl());
            holder = PrintExtensions.AddBytes(holder, Encoding.ASCII.GetBytes(alamat + "\n"));
            holder = PrintExtensions.AddBytes(holder, esCmd.Lf());
            return this;

        }

        public RawPrinterHelper CetakTanggal(string tanggal)
        {
            holder = PrintExtensions.AddBytes(holder, esCmd.Alignment.Right());
            holder = PrintExtensions.AddBytes(holder, tanggal + "\n");
            return this;
        }

        public RawPrinterHelper CetakNama(string nama)
        {
            holder = PrintExtensions.AddBytes(holder, esCmd.Lf());
            holder = PrintExtensions.AddBytes(holder, esCmd.Alignment.Left());
            holder = PrintExtensions.AddBytes(holder, Encoding.ASCII.GetBytes(string.Format("Nama Pembeli : {0} \n", nama)));
            return this;
        }

        public RawPrinterHelper CetakKodeBelanja(string kode)
        {
            holder = PrintExtensions.AddBytes(holder, Encoding.ASCII.GetBytes("-------------------------"));
            holder = PrintExtensions.AddBytes(holder, esCmd.Lf());
            holder = PrintExtensions.AddBytes(holder, Encoding.ASCII.GetBytes(String.Format("Kode : {0}-" + DateTime.Now.ToString("dd/MM/yyyy"), kode)));
            holder = PrintExtensions.AddBytes(holder, esCmd.Lf());
            holder = PrintExtensions.AddBytes(holder, Encoding.ASCII.GetBytes("-------------------------"));
            holder = PrintExtensions.AddBytes(holder, esCmd.Lf());
            return this;
        }

        public RawPrinterHelper CetakBelanjaan(List<BarangBelanjaan> item)
        {

            foreach (BarangBelanjaan items in item)
            {
                holder = PrintExtensions.AddBytes(holder, Encoding.ASCII.GetBytes(items.NamaBarang + "\n"));
                holder = PrintExtensions.AddBytes(holder, Encoding.ASCII.GetBytes($"{items.JumlahBeli.ToString()} {items.JenisKuantitas}"));

                holder = PrintExtensions.AddBytes(holder, Encoding.ASCII.GetBytes("    "));
                holder = PrintExtensions.AddBytes(holder, Encoding.ASCII.GetBytes(items.HargaProduk.ToString()));
                holder = PrintExtensions.AddBytes(holder, Encoding.ASCII.GetBytes("     "));
                holder = PrintExtensions.AddBytes(holder, Encoding.ASCII.GetBytes(items.HargaJumlahProduk.ToString()));
                holder = PrintExtensions.AddBytes(holder, esCmd.Lf());
                holder = PrintExtensions.AddBytes(holder, esCmd.Lf());
            }

            return this;
        }

        public RawPrinterHelper CetakUang(string total, string bayar, string kembali)
        {
            holder = PrintExtensions.AddBytes(holder, esCmd.Alignment.Right());
            holder = PrintExtensions.AddBytes(holder, Encoding.ASCII.GetBytes("-------------------------"));
            holder = PrintExtensions.AddBytes(holder, esCmd.Lf());
            holder = PrintExtensions.AddBytes(holder, Encoding.ASCII.GetBytes(string.Format("Jumlah :     {0}\n", total)));
            holder = PrintExtensions.AddBytes(holder, Encoding.ASCII.GetBytes(string.Format("Bayar :     {0}\n", bayar)));
            holder = PrintExtensions.AddBytes(holder, Encoding.ASCII.GetBytes(string.Format("Kembalian :     {0}", kembali)));
            holder = PrintExtensions.AddBytes(holder, esCmd.Lf());
            holder = PrintExtensions.AddBytes(holder, Encoding.ASCII.GetBytes("-------------------------"));
            return this;
        }

        public void PrintDirect(string printerName)
        {
            holder = PrintExtensions.AddBytes(holder, esCmd.Lf());
            holder = PrintExtensions.AddBytes(holder, esCmd.Lf());
            holder = PrintExtensions.AddBytes(holder, esCmd.Alignment.Center());
            holder = PrintExtensions.AddBytes(holder, Encoding.ASCII.GetBytes("---Terima Kasih---"));
            holder = PrintExtensions.AddBytes(holder, esCmd.Lf());
            holder = PrintExtensions.AddBytes(holder, esCmd.Lf());
            holder = PrintExtensions.AddBytes(holder, esCmd.Lf());
            holder = PrintExtensions.AddBytes(holder, esCmd.Alignment.Left());
            holder = PrintExtensions.AddBytes(holder, esCmd.CharSize.Nomarl());
            IPrinter print = new Printer();
            MemoryStream stream = new MemoryStream();
            stream.Write(holder, 0, holder.Length);
            print.PrintRawStream(printerName, stream, "PrintToko", false);
        }
    }
}

