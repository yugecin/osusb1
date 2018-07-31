using System;
using System.IO;
using System.Text;

namespace osusb1 {
partial class all {
	class Font {
		public byte charheight;
		public byte[] charwidth;
		public byte[][] chardata;

		public Font() {
			charwidth = new byte[96];
			chardata = new byte[96][];
			using (BinaryReader b = new BinaryReader(new FileStream("font.txt", FileMode.Open))) {
				charheight = b.ReadByte();
				for (int i = 0; i < 96; i++) {
					charwidth[i] = b.ReadByte();
					chardata[i] = new byte[charheight];
					for (int j = 0; j < charheight; j++) {
						chardata[i][j] = b.ReadByte();
					}
				}			 
			}
		}

		public int textWidth(string text) {
			int width = 0;
			for (int i = 0; i < text.Length; i++) {
				char c = text[i];
				if (32 <= c && c <= 128) {
					width += charwidth[c - 32] + 1;
				}
			}
			if (text.Length > 0) {
				width -= 1;
			}
			return width;
		}
	}
}
}
