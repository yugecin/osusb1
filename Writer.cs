using System;
using System.IO;
using System.Text;

namespace osusb1 {
	public class Writer {
		
		public StreamWriter w;

		public Writer(StreamWriter w) {
			this.w = w;
		}

		public void Sprite(string origin, string sprite, int x, int y) {
			w.Write("Sprite,Foreground,{0},{1},{2},{3}\n", origin, sprite, x, y);
		}

		// TODO: minimize decimals here?
		public void _F(int starttime, int endtime, float startopacity, float endopacity) {
			if (startopacity == endopacity) {
				return;
			}
			w.Write("_F,0,{0},{1},{2},{3}\n", starttime, endtime, startopacity, endopacity);
		}

		public void _M(int starttime, int endtime, int startx, int starty, int endx, int endy) {
			if (startx == endx) {
				_MY(starttime, endtime, starty, endy);
				return;
			}
			if (starty == endy) {
				_MX(starttime, endtime, startx, endx);
				return;
			}
			w.Write("_M,0,{0},{1},{2},{3},{4},{5}\n", starttime, endtime, startx, starty, endx, endy);
		}

		public void _MX(int starttime, int endtime, int startx, int endx) {
			if (startx == endx) {
				return;
			}
			w.Write("_MX,0,{0},{1},{2},{3}\n", starttime, endtime, startx, endx);
		}

		public void _MY(int starttime, int endtime, int starty, int endy) {
			if (starty == endy) {
				return;
			}
			w.Write("_MY,0,{0},{1},{2},{3}\n", starttime, endtime, starty, endy);
		}

		// TODO: minimize decimals here?
		public void _S(int starttime, int endtime, float startscale, float endscale) {
			if (startscale == endscale) {
				return;
			}
			w.Write("_S,0,{0},{1},{2},{3}\n", starttime, endtime, startscale, endscale);
		}

		// TODO: minimize decimals here?
		public void _V(int starttime, int endtime, float startscalex, float startscaley, float endscalex, float endscaley) {
			if (startscalex == endscalex && startscaley == endscaley) {
				return;
			}
			w.Write("_V,0,{0},{1},{2},{3},{4},{5}\n", starttime, endtime, startscalex, startscaley, endscalex, endscaley);
		}

		// TODO: minimize decimals here?
		public void _R(int starttime, int endtime, float startrotate, float endrotate) {
			if (startrotate == endrotate) {
				return;
			}
			w.Write("_R,0,{0},{1},{2},{3}\n", starttime, endtime, startrotate, endrotate);
		}

		public void ln(string line) {
			w.Write(line + "\n");
		}

	}
}
