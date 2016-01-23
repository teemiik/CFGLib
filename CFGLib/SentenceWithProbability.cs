﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	/// <summary>
	/// Represents a sentence together with its associated probability of being generated
	/// </summary>
	public class SentenceWithProbability {
		public double Probability;
		public Sentence Sentence;

		public SentenceWithProbability(double v, Sentence s) {
			this.Probability = v;
			this.Sentence = s;
		}
		public override string ToString() {
			return string.Format("{0}: {1}", Probability, Sentence);
		}
	}
}
