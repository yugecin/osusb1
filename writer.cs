using System;
using System.IO;
using System.Text;

namespace osusb1 {
partial class all {
	class Writer {
		public StreamWriter w;

		public int byteswritten;
		bool comments;

		public Writer(StreamWriter w, bool comments) {
			this.w = w;
			this.comments = comments;
		}

		public void ln(string line) {
			byteswritten += line.Length + 1;
			w.Write(line + "\n");
		}

		public void comment(string line) {
			if (comments) {
				ln("// " + line);
			}
		}
	}
}
}
