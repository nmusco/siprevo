﻿using System;
using System.Collections.Generic;

namespace Fsm
{
	class IntSet
	{
		private List<LinkedList<DfaState>> _sets;

		public IntSet(List<DfaState> states)
		{
			_sets = new List<LinkedList<DfaState>>(100000);

			_sets.Add(new LinkedList<DfaState>());

			foreach (var state in states)
			{
				state.SetId = 0;
				_sets[0].AddLast(state);
			}
		}

		public DfaState GetFirst(int setId)
		{
			if (setId < _sets.Count)
				return _sets[setId].First.Value;

			return null;
		}

		public IEnumerable<LinkedListNode<DfaState>> GetSet(int setId)
		{
			if (setId < _sets.Count)
			{
				for (LinkedListNode<DfaState> next, current = _sets[setId].First; 
					current != null; current = next)
				{
					next = current.Next;
					yield return current;
				}
			}
		}

		public void MoveToSet(LinkedListNode<DfaState> current, int setId)
		{
			while (_sets.Count <= setId)
				_sets.Add(new LinkedList<DfaState>());

			_sets[current.Value.SetId].Remove(current);

			current.Value.SetId = setId;
			_sets[current.Value.SetId].AddFirst(current);
		}

		public int Count
		{
			get { return _sets.Count; }
		}
	}
}