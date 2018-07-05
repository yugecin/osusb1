using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace osusb1 {
partial class all {

	class Writer {
		
		public StreamWriter w;

		public bool check;

		public Writer(StreamWriter w) {
			this.w = w;
			this.check = true;
		}

		public void Sprite(string origin, string sprite, int x, int y) {
			w.Write("Sprite,Foreground,{0},{1},{2},{3}\n", origin, sprite, x, y);
		}

		// TODO: minimize decimals here?
		public void _F(int starttime, int endtime, float startopacity, float endopacity) {
			w.Write("_F,0,{0},{1},{2},{3}\n", starttime, endtime, startopacity, endopacity);
		}

		// TODO: minimize decimals here?
		public void _Fi(int time, float from, float to) {
			w.Write("_F,0,{0},{0},{1},{2}\n", time, from, to);
		}

		public void _M(int starttime, int endtime, int startx, int starty, int endx, int endy) {
			if (check && startx == endx) {
				_MY(starttime, endtime, starty, endy);
				return;
			}
			if (check && starty == endy) {
				_MX(starttime, endtime, startx, endx);
				return;
			}
			w.Write("_M,0,{0},{1},{2},{3},{4},{5}\n", starttime, endtime, startx, starty, endx, endy);
		}

		public void _MX(int starttime, int endtime, int startx, int endx) {
			if (check && startx == endx) {
				return;
			}
			w.Write("_MX,0,{0},{1},{2},{3}\n", starttime, endtime, startx, endx);
		}

		public void _MY(int starttime, int endtime, int starty, int endy) {
			if (check && starty == endy) {
				return;
			}
			w.Write("_MY,0,{0},{1},{2},{3}\n", starttime, endtime, starty, endy);
		}

		// TODO: minimize decimals here?
		public void _S(int starttime, int endtime, float startscale, float endscale) {
			if (check && startscale == endscale) {
				return;
			}
			w.Write("_S,0,{0},{1},{2},{3}\n", starttime, endtime, startscale, endscale);
		}

		// TODO: minimize decimals here?
		public void _V(int starttime, int endtime, float startscalex, float startscaley, float endscalex, float endscaley) {
			if (check && startscalex == endscalex && startscaley == endscaley) {
				return;
			}
			w.Write("_V,0,{0},{1},{2},{3},{4},{5}\n", starttime, endtime, startscalex, startscaley, endscalex, endscaley);
		}

		// TODO: minimize decimals here?
		public void _R(int starttime, int endtime, float startrotate, float endrotate) {
			if (check && startrotate == endrotate) {
				return;
			}
			w.Write("_R,0,{0},{1},{2},{3}\n", starttime, endtime, startrotate, endrotate);
		}

		public void _C(int starttime, int endtime, Color startcolor, Color endcolor) {
			if (check && startcolor == endcolor) {
				return;
			}
			w.Write("_C,0,{0},{1},{2},{3},{4},{5},{6},{7}\n", starttime, endtime, startcolor.R, startcolor.G, startcolor.B,
				endcolor.R, endcolor.G, endcolor.B);
		}

		public void _Ci(int time, Color color) {
			w.Write("_C,0,{0},{0},0,0,0,{1},{2},{3}\n", time, color.R, color.G, color.B);
		}

		public void ln(string line) {
			w.Write(line + "\n");
		}

	}
}
}
