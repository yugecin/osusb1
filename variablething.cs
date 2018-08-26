﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace osusb1 {
partial class all {

	class RP {
		public string search;
		public float occurences;
		public float rate;
		public string replacement;
		public RP(string search) {
			this.search = search;
			occurences = 0;
			rate = 0f;
		}
	}

	class RPCMPRATE : Comparer<RP> {
		public override int Compare(RP a, RP o) {
			return o.rate.CompareTo(a.rate);
		}
	}

	class RPCMPLEN : Comparer<RP> {
		public override int Compare(RP a, RP o) {
			return o.search.Length.CompareTo(a.search.Length);
		}
	}

	class RPCMPVLEN : Comparer<RP> {
		public override int Compare(RP a, RP o) {
			return a.replacement.Length.CompareTo(o.replacement.Length);
		}
	}

	internal
	static void dovariablething() {
		List<RP> replacements = new List<RP>(); 
		replacements.Add(new RP("__F,0,0,100,1"));
		replacements.Add(new RP("__F,0,100,200,0"));
		replacements.Add(new RP("4,3,1,d,0,0"));
		replacements.Add(new RP("4,3,8,t,0,0"));
		replacements.Add(new RP("4,3,1,1,0,0"));
		replacements.Add(new RP("4,3,1,2,0,0"));
		replacements.Add(new RP("4,3,1,3,0,0"));
		replacements.Add(new RP("4,3,1,,0,0"));
		replacements.Add(new RP(",255,255,255"));
		replacements.Add(new RP(",0,0,0"));
		for (int i = 10; i < 100; i++) {
			replacements.Add(new RP("_M,0," + i));
			replacements.Add(new RP("_C,0," + i));
			replacements.Add(new RP("_R,0," + i));
			replacements.Add(new RP("_V,0," + i));
			replacements.Add(new RP("_S,0," + i));
			replacements.Add(new RP("_F,0," + i));
		}
		using (StreamReader r = new StreamReader(osbx)) {
			string s;
			while ((s = r.ReadLine()) != null) {
				foreach (RP rp in replacements) {
					if (s.IndexOf(rp.search) != -1) {
						rp.occurences++;
					}
				}
			}
		}
		foreach (RP rp in replacements) {
			rp.rate = rp.occurences * (rp.search.Length - 2);
		}
		replacements.Sort(new RPCMPRATE());
		char v = (char) 0;
		foreach (RP rp in replacements) {
			while (v == '$' || v == '\n' || v == '\r' || v == ',' ||
				v == '=' || ('0' <= v && v <= '9'))
			{
				v++;
			}
			rp.replacement = "$" + v;
			v++;
		}
		replacements.Sort(new RPCMPLEN());
		var sb = new StringBuilder();
		using (StreamReader r = new StreamReader(osbx)) {
			string s;
			while ((s = r.ReadLine()) != null) {
				if (s.Length == 0) {
					continue;
				}
				foreach (RP rp in replacements) {
					s = s.Replace(rp.search, rp.replacement);
				}
				sb.Append(s);
				sb.Append('\n');
			}
		}
		using (StreamWriter w = new StreamWriter(osb)) {
			w.Write("[256]\n");
			replacements.Sort(new RPCMPVLEN());
			foreach (RP rp in replacements) {
				w.Write(rp.replacement + "=" + rp.search + "\n");
			}
			w.Write("$,=,\n");
			w.Write("$,$,=1\n");
			w.Write("$,$,$,=2\n");
			w.Write("$,$,$,$,=3\n");
			w.Write("$,$,$,$,$,=4\n");
			w.Write("$,$,$,$,$,$,=5\n");
			w.Write("$,$,$,$,$,$,$,=6\n");
			w.Write("$,$,$,$,$,$,$,$,=7\n");
			w.Write("$,$,$,$,$,$,$,$,$,=8\n");
			w.Write("$,$,$,$,$,$,$,$,$,$,=9\n");
			w.Write("$,$,$,$,$,$,$,$,$,$,$,=0\n");
			w.Write(sb.ToString());
		}
		replacements.Sort(new RPCMPRATE());
		Console.WriteLine("variables");
		Console.WriteLine("---------");
		foreach (RP rp in replacements) {
			Console.WriteLine("{0,10}: rate {1,6} x{2,6}", rp.search, rp.rate, rp.occurences);
		}
	}
}
}
