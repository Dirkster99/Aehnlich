namespace Menees.Diffs
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Drawing;

	#endregion

	/// <summary>
	/// This class implements the differencing algorithm from
	/// "An O(ND) Difference Algorithm And Its Variations" by
	/// Eugene W. Myers.  It is the standard algorithm used by
	/// the UNIX diff utilities.
	///
	/// This implementation diffs two comparable lists.  It is
	/// typically used with lists of hash values where each
	/// hash corresponds to a line of text.  Then it can be
	/// used to diff two text files on a line-by-line basis.
	/// </summary>
	public sealed class MyersDiff<T> where T : IComparable<T>
	{
		#region Private Data Members

		private IList<T> listA; // Sequence A
		private IList<T> listB; // Sequence B
		private bool supportChangeEditType;
		private DiagonalVector vectorForward;
		private DiagonalVector vectorReverse;

		#endregion

		#region Constructors

		public MyersDiff(IList<T> listA, IList<T> listB, bool supportChangeEditType)
		{
			this.listA = listA;
			this.listB = listB;
			this.supportChangeEditType = supportChangeEditType;

			int n = listA.Count;
			int m = listB.Count;

			this.vectorForward = new DiagonalVector(n, m);
			this.vectorReverse = new DiagonalVector(n, m);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Returns an EditScript instance that gives all the Edits
		/// necessary to transform A into B.
		/// </summary>
		public EditScript Execute()
		{
			List<Point> matchPoints = new List<Point>();

			SubArray<T> subArrayA = new SubArray<T>(this.listA);
			SubArray<T> subArrayB = new SubArray<T>(this.listB);

			this.GetMatchPoints(subArrayA, subArrayB, matchPoints);
			Debug.Assert(matchPoints.Count == this.GetLongestCommonSubsequenceLength(), "The number of match points must equal the LCS length.");

			EditScript result = this.ConvertMatchPointsToEditScript(subArrayA.Length, subArrayB.Length, matchPoints);
			Debug.Assert(result.TotalEditLength == this.GetShortestEditScriptLength(), "The total edit length must equal the SES length.");

			return result;
		}

		/// <summary>
		/// Returns the longest common subsequence from A and B.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",
			Justification = "This performs a long, complex calculation.")]
		public IList<T> GetLongestCommonSubsequence()
		{
			List<T> result = new List<T>();

			this.GetLcs(new SubArray<T>(this.listA), new SubArray<T>(this.listB), result);

			return result;
		}

		/// <summary>
		/// Calculates the length that the LCS should be without
		/// actually determining the LCS.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",
			Justification = "This performs a long, complex calculation.")]
		public int GetLongestCommonSubsequenceLength()
			/* Per Myers's paper, we should always have D+2L == N+M.  So L == (N+M-D)/2. */
			=> (this.listA.Count + this.listB.Count - this.GetShortestEditScriptLength()) / 2;

		/// <summary>
		/// Gets the length of the "shortest edit script"
		/// by running the algorithm in reverse.  We should
		/// always have GetSESLength() == GetReverseSESLength().
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",
			Justification = "This performs a long, complex calculation.")]
		public int GetReverseShortestEditScriptLength()
		{
			SubArray<T> subArrayA = new SubArray<T>(this.listA);
			SubArray<T> subArrayB = new SubArray<T>(this.listB);

			if (this.SetupFictitiousPoints(subArrayA, subArrayB))
			{
				int n = this.listA.Count;
				int m = this.listB.Count;
				int delta = n - m;

				for (int d = 0; d <= n + m; d++)
				{
					for (int k = -d; k <= d; k += 2)
					{
						int x = this.GetReverseDPaths(subArrayA, subArrayB, d, k, delta);
						int y = x - (k + delta);
						if (x <= 0 && y <= 0)
						{
							return d;
						}
					}
				}

				// We should never get here if the algorithm is coded correctly.
				Debug.Assert(false, "This code should be unreachable.");
				return -1;
			}
			else if (this.listA.Count == 0)
			{
				return this.listB.Count;
			}
			else
			{
				return this.listA.Count;
			}
		}

		/// <summary>
		/// Calculates the length of the "shortest edit script"
		/// as defined in Myers's paper.
		///
		/// Note: This may not be the same as the Count property
		/// of an EditScript instance returned by Execute().  If
		/// an EditScript instance has any Edits with Length > 1,
		/// then those groupings will make EditScript.Count less
		/// than GetSESLength().  Similarly, an Edit with EditType
		/// Change should be thought of as a combined Delete and
		/// Insert for the specified Length.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",
			Justification = "This performs a long, complex calculation.")]
		public int GetShortestEditScriptLength()
		{
			SubArray<T> subArrayA = new SubArray<T>(this.listA);
			SubArray<T> subArrayB = new SubArray<T>(this.listB);

			if (this.SetupFictitiousPoints(subArrayA, subArrayB))
			{
				int n = this.listA.Count;
				int m = this.listB.Count;

				for (int d = 0; d <= n + m; d++)
				{
					for (int k = -d; k <= d; k += 2)
					{
						int x = this.GetForwardDPaths(subArrayA, subArrayB, d, k);
						int y = x - k;
						if (x >= n && y >= m)
						{
							return d;
						}
					}
				}

				// We should never get here if the algorithm is coded correctly.
				Debug.Assert(false, "This code should be unreachable.");
				return -1;
			}
			else if (this.listA.Count == 0)
			{
				return this.listB.Count;
			}
			else
			{
				return this.listA.Count;
			}
		}

		/// <summary>
		/// Returns a similary index between 0 and 1 inclusive.
		/// 0 means A and B are completely different.  1 means
		/// they are exactly alike.  The similarity index is
		/// calculated as twice the length of the LCS divided
		/// by the sum of A and B's lengths.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",
			Justification = "This performs a long, complex calculation.")]
		public double GetSimilarity()
		{
			double result = GetSimilarity(this.listA.Count, this.listB.Count, this.GetLongestCommonSubsequenceLength());
			return result;
		}

		#endregion

		#region Private Methods

		private static double GetSimilarity(int lengthA, int lengthB, int lengthLcs)
		{
			double result = (2.0 * lengthLcs) / (double)(lengthA + lengthB);
			return result;
		}

		private EditScript ConvertMatchPointsToEditScript(int n, int m, List<Point> matchPoints)
		{
			// The Execute method already has an assertion that the number of MatchPoints
			// equals the LCS length, so we can use it to calculate similarity, but we must do
			// it before we add a fictitious match point below.
			double similarity = GetSimilarity(n, m, matchPoints.Count);
			EditScript script = new EditScript(similarity);

			int currentX = 1;
			int currentY = 1;

			// Add a fictitious match point at (N+1, M+1) so we're guaranteed to
			// pick up all edits with a single loop.
			matchPoints.Add(new Point(n + 1, m + 1));

			// NOTE: When we create new Edit instances, we'll store iCurrX and iCurrY
			// minus 1 because we want to convert them back to 0-based indexes for
			// the user.  The user shouldn't have to know that internally we use any
			// 1-based types.
			foreach (Point point in matchPoints)
			{
				int matchX = point.X;
				int matchY = point.Y;

				// A one-to-one grouping of inserts and deletes will be considered a change.
				if (this.supportChangeEditType && currentX < matchX && currentY < matchY)
				{
					int changeLength = Math.Min(matchX - currentX, matchY - currentY);
					script.Add(new Edit(EditType.Change, currentX - 1, currentY - 1, changeLength));
					currentX += changeLength;
					currentY += changeLength;
				}

				if (currentX < matchX)
				{
					script.Add(new Edit(EditType.Delete, currentX - 1, currentY - 1, matchX - currentX));
				}

				if (currentY < matchY)
				{
					script.Add(new Edit(EditType.Insert, currentX - 1, currentY - 1, matchY - currentY));
				}

				currentX = matchX + 1;
				currentY = matchY + 1;
			}

			return script;
		}

		private int FindMiddleSnake(SubArray<T> subArrayA, SubArray<T> subArrayB, out int pathStartX, out int pathEndX, out int pathK)
		{
			// We don't have to check the result of this because the calling procedure
			// has already check the length preconditions.
			this.SetupFictitiousPoints(subArrayA, subArrayB);

			pathStartX = -1;
			pathEndX = -1;
			pathK = 0;

			int delta = subArrayA.Length - subArrayB.Length;
			int ceiling = (int)Math.Ceiling((subArrayA.Length + subArrayB.Length) / 2.0);

			for (int d = 0; d <= ceiling; d++)
			{
				for (int k = -d; k <= d; k += 2)
				{
					// Find the end of the furthest reaching forward D-path in diagonal k.
					this.GetForwardDPaths(subArrayA, subArrayB, d, k);

					// If delta is odd (i.e. remainder == 1 or -1) and ...
					if ((delta % 2 != 0) && (k >= (delta - (d - 1)) && k <= (delta + (d - 1))))
					{
						// If the path overlaps the furthest reaching reverse (D-1)-path in diagonal k.
						if (this.vectorForward[k] >= this.vectorReverse[k])
						{
							// The last snake of the forward path is the middle snake.
							pathK = k;
							pathEndX = this.vectorForward[k];
							pathStartX = pathEndX;
							int pathStartY = pathStartX - pathK;
							while (pathStartX > 0 && pathStartY > 0 && subArrayA[pathStartX].CompareTo(subArrayB[pathStartY]) == 0)
							{
								pathStartX--;
								pathStartY--;
							}

							// Length of an SES is 2D-1.
							return (2 * d) - 1;
						}
					}
				}

				for (int k = -d; k <= d; k += 2)
				{
					// Find the end of the furthest reaching reverse D=path in diagonal k+iDelta
					this.GetReverseDPaths(subArrayA, subArrayB, d, k, delta);

					// If iDelta is even and ...
					if ((delta % 2 == 0) && ((k + delta) >= -d && (k + delta) <= d))
					{
						// If the path overlaps the furthest reaching forward D-path in diagonal k+iDelta.
						if (this.vectorReverse[k + delta] <= this.vectorForward[k + delta])
						{
							// The last snake of the reverse path is the middle snake.
							pathK = k + delta;
							pathStartX = this.vectorReverse[pathK];
							pathEndX = pathStartX;
							int pathEndY = pathEndX - pathK;
							while (pathEndX < subArrayA.Length &&
								pathEndY < subArrayB.Length &&
								subArrayA[pathEndX + 1].CompareTo(subArrayB[pathEndY + 1]) == 0)
							{
								pathEndX++;
								pathEndY++;
							}

							// Length of an SES is 2D.
							return 2 * d;
						}
					}
				}
			}

			// We should never get here if the algorithm is coded correctly.
			Debug.Assert(false, "This code should be unreachable.");
			return -1;
		}

		private int GetForwardDPaths(SubArray<T> subArrayA, SubArray<T> subArrayB, int d, int k)
		{
			DiagonalVector vector = this.vectorForward;

			int x;
			if ((k == -d) || (k != d && vector[k - 1] < vector[k + 1]))
			{
				x = vector[k + 1];
			}
			else
			{
				x = vector[k - 1] + 1;
			}

			int y = x - k;

			while (x < subArrayA.Length && y < subArrayB.Length && subArrayA[x + 1].CompareTo(subArrayB[y + 1]) == 0)
			{
				x++;
				y++;
			}

			vector[k] = x;

			return x;
		}

		private void GetLcs(SubArray<T> subArrayA, SubArray<T> subArrayB, List<T> output)
		{
			if (subArrayA.Length > 0 && subArrayB.Length > 0)
			{
				// Find the length D and the middle snake from (x,y) to (u,v)
				int x, u, k;
				int d = this.FindMiddleSnake(subArrayA, subArrayB, out x, out u, out k);
				int y = x - k;
				int v = u - k;

				if (d > 1)
				{
					this.GetLcs(new SubArray<T>(subArrayA, 1, x), new SubArray<T>(subArrayB, 1, y), output);

					for (int i = x + 1; i <= u; i++)
					{
						output.Add(subArrayA[i]);
					}

					this.GetLcs(new SubArray<T>(subArrayA, u + 1, subArrayA.Length - u), new SubArray<T>(subArrayB, v + 1, subArrayB.Length - v), output);
				}
				else if (subArrayB.Length > subArrayA.Length)
				{
					for (int i = 1; i <= subArrayA.Length; i++)
					{
						output.Add(subArrayA[i]);
					}
				}
				else
				{
					for (int i = 1; i <= subArrayB.Length; i++)
					{
						output.Add(subArrayB[i]);
					}
				}
			}
		}

		private void GetMatchPoints(SubArray<T> subArrayA, SubArray<T> subArrayB, List<Point> matchPoints)
		{
			if (subArrayA.Length > 0 && subArrayB.Length > 0)
			{
				// Find the middle snake from (x,y) to (u,v)
				int x, u, k;
				int d = this.FindMiddleSnake(subArrayA, subArrayB, out x, out u, out k);
				int y = x - k;
				int v = u - k;

				if (d > 1)
				{
					this.GetMatchPoints(new SubArray<T>(subArrayA, 1, x), new SubArray<T>(subArrayB, 1, y), matchPoints);

					for (int i = x + 1; i <= u; i++)
					{
						// Output absolute X and Y (not relative to the current subarray)
						matchPoints.Add(new Point(i + subArrayA.Offset, i - k + subArrayB.Offset));
					}

					this.GetMatchPoints(
						new SubArray<T>(subArrayA, u + 1, subArrayA.Length - u),
						new SubArray<T>(subArrayB, v + 1, subArrayB.Length - v),
						matchPoints);
				}
				else
				{
					// If there are no differences, we have to output all of the points.
					// If there's only one difference, we have to output all of the
					// match points, skipping the single point that is different.
					Debug.Assert(d == 0 || Math.Abs(subArrayA.Length - subArrayB.Length) == 1, "A and B's lengths must differ by 1 if D == 1");

					// Only go to the minimum of the two lengths since that's the
					// most that can possibly match between the two subsequences.
					int n = subArrayA.Length;
					int m = subArrayB.Length;
					if (m > n)
					{
						// Output A[1..N] as match points
						int currentY = 1;
						for (int i = 1; i <= n; i++)
						{
							// We must skip the one difference when we hit it
							if (subArrayA[i].CompareTo(subArrayB[currentY]) != 0)
							{
								currentY++;
							}

							matchPoints.Add(new Point(i + subArrayA.Offset, currentY + subArrayB.Offset));
							currentY++;
						}
					}
					else
					{
						// Output B[1..M] as match points
						int currentX = 1;
						for (int i = 1; i <= m; i++)
						{
							// We must skip the one difference when we hit it
							if (subArrayA[currentX].CompareTo(subArrayB[i]) != 0)
							{
								currentX++;
							}

							matchPoints.Add(new Point(currentX + subArrayA.Offset, i + subArrayB.Offset));
							currentX++;
						}
					}
				}
			}
		}

		private int GetReverseDPaths(SubArray<T> subArrayA, SubArray<T> subArrayB, int d, int k, int delta)
		{
			DiagonalVector vector = this.vectorReverse;

			int p = k + delta;

			int x;
			if ((k == -d) || (k != d && vector[p + 1] <= vector[p - 1]))
			{
				x = vector[p + 1] - 1;
			}
			else
			{
				x = vector[p - 1];
			}

			int y = x - p;

			while (x > 0 && y > 0 && subArrayA[x].CompareTo(subArrayB[y]) == 0)
			{
				x--;
				y--;
			}

			vector[p] = x;

			return x;
		}

		private bool SetupFictitiousPoints(SubArray<T> subArrayA, SubArray<T> subArrayB)
		{
			if (subArrayA.Length > 0 && subArrayB.Length > 0)
			{
				// Setup some "fictious" endpoints for initial forward
				// and reverse path navigation.
				this.vectorForward[1] = 0;
				int n = subArrayA.Length;
				int m = subArrayB.Length;
				int delta = n - m;
				this.vectorReverse[delta + 1] = n + 1;

				return true;
			}
			else
			{
				return false;
			}
		}

		#endregion

		#region Private Types

		// Declare this so we don't have to reference System.Drawing.dll, which shouldn't be used in Windows services.
		private struct Point
		{
			#region Constructors

			public Point(int x, int y)
				: this()
			{
				this.X = x;
				this.Y = y;
			}

			#endregion

			#region Public Properties

			public int X { get; }

			public int Y { get; }

			#endregion
		}

		#endregion
	}
}
