﻿using System;
using System.Collections.Generic;

namespace CFGLib {
	public class Nonterminal : Word {
		private static Dictionary<string, Nonterminal> _history = new Dictionary<string, Nonterminal>();

		private readonly string _name;

		private Nonterminal(string name) {
			_name = name;
		}

		public static Nonterminal Of(string v) {
			Nonterminal nonterminal;
			if (!_history.TryGetValue(v, out nonterminal)) {
				nonterminal = new Nonterminal(v);
				_history[v] = nonterminal;
			}
			return nonterminal;
		}

		public override string ToString() {
			return string.Format("Var({0})", _name);
		}

		public Sentence ProduceBy(Grammar grammar) {
			return grammar.ProduceNonterminal(this);
		}
		public bool IsNonterminal() {
			return true;
		}
	}
}