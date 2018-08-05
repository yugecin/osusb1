using System;
using System.IO;
using System.Text;

namespace osusb1 {
partial class all {
	class Writer {
		public StreamWriter w;

		public bool check;
		public int byteswritten;

		public Writer(StreamWriter w) {
			this.w = w;
			this.check = true;
		}

		public void ln(string line) {
			byteswritten += line.Length + 1;
			w.Write(line + "\n");
		}
	}
}
}
