﻿using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Fsm;

namespace SipDfaCompiler
{
	class ActionsDescription
	{
		private static Regex _regex =
			new Regex(@"^(?<func>Const|Range|ContinueRange|Count|Decimal|Bool|Group|Hex|Reset|ResetIfInvalid)(\s*=(\s*(?<arg1>[\<\>\?\[\]\.A-Za-z0-9_\-]*)(\s*,\s*(?<arg2>[\<\>\?\[\]\.A-Za-z0-9_\-]+))?(\s*,\s*(?<arg3>[\?\[\]\.A-Za-z0-9_\-]*))?(\s*,\s*(?<arg4>[\?\[\]\.A-Za-z0-9_\-]+))?(\s*,\s*(?<arg5>[\?\[\]\.A-Za-z0-9_\-]+))?))?;",
				RegexOptions.IgnoreCase);

		public static ActionsDescription TryParse(string description, string path)
		{
			if (description != "" && description.TrimEnd().EndsWith(";") == false)
				description += ";";

			var reservArgs = path.Split('.');
			var actions = new List<Action>();

			var match = _regex.Match(description);
			while (match.Success)
			{
				Action action = null;
				try
				{
					switch (match.Groups["func"].Value)
					{
						case "Const":
							action = new Action(Marks.Const, 3);
							SetArg(action, 0, match, reservArgs, 2);
							SetArg(action, 1, match, reservArgs, 2);
							SetArg(action, 2, match, "10");
							break;
						case "Range":
							action = new Action(Marks.BeginRange, 1);
							SetArg(action, 0, match, reservArgs, 1);
							break;
						case "ContinueRange":
							action = new Action(Marks.ContinueRange, 1);
							SetArg(action, 0, match, reservArgs, 1);
							break;
						case "Reset":
							action = new Action(Marks.ResetRange, 1);
							SetArg(action, 0, match, reservArgs, 1);
							break;
						case "ResetIfInvalid":
							action = new Action(Marks.ResetRangeIfInvalid, 1);
							SetArg(action, 0, match, reservArgs, 1);
							break;
						case "Count":
							action = new Action(Marks.Count, 3);
							SetArg(action, 0, match, reservArgs, 1);
							SetArg(action, 1, match, "10");
							SetArg(action, 2, match, "0");
							break;
						case "Decimal":
							action = new Action(Marks.Decimal, 1);
							SetArg(action, 0, match, reservArgs, 1);
							break;
						case "Hex":
							action = new Action(Marks.Hex, 1);
							SetArg(action, 0, match, reservArgs, 1);
							break;
						case "Bool":
							action = new Action(Marks.Bool, 1);
							SetArg(action, 0, match, reservArgs, 1);
							break;
						case "Group":
							action = new Action(Marks.Group, 2);
							SetArg(action, 0, match, "");
							SetArg(action, 1, match, "NoneStruct");
							break;
						default:
							throw new Exception();
					}
				}
				catch
				{
					return null;
				}

				actions.Add(action);

				match = match.NextMatch();
			}

			var result = new ActionsDescription() { Description = description, };

			if (actions != null && actions.Count > 0)
				result.Actions = actions.ToArray();

			if (result.IsEmpty == false && result.Actions == null)
				return null;

			return result;
		}

		private static void SetArg(Action action, int i, Match match, string[] reservArgs, int reservOver)
		{
			var value = match.Groups["arg" + (i + 1)].Value;
			if (string.IsNullOrEmpty(value))
				value = ToCsName(reservArgs[reservArgs.Length - reservOver + i]);

			action.Args[i] = value;
		}

		private static void SetArg(Action action, int i, Match match, string defaultV)
		{
			var value = match.Groups["arg" + (i + 1)].Value;
			if (string.IsNullOrEmpty(value))
				value = ToCsName(defaultV);

			action.Args[i] = value;
		}

		public string Description
		{
			get;
			private set;
		}

		public Action[] Actions
		{
			get;
			private set;
		}

		public bool IsEmpty
		{
			get { return string.IsNullOrEmpty(Description); }
		}

		public int UseCount
		{
			get;
			set;
		}

		public ActionsDescription Clone(string oldValue, string newValue)
		{
			var actionDescription = new ActionsDescription()
			{
				Actions = new Action[Actions.Length],
				Description = "// GENERATED",
			};

			for (int i = 0; i < Actions.Length; i++)
				actionDescription.Actions[i] = Actions[i].Clone(oldValue, newValue);

			return actionDescription;
		}

		public class Action
		{
			public Action(Marks mark, int size)
			{
				Mark = mark;
				Args = new string[size];
			}

			public Action Clone(string oldValue, string newValue)
			{
				var action = new Action(Mark, Args.Length);

				for (int i = 0; i < Args.Length; i++)
					action.Args[i] = Args[i].Replace(oldValue, newValue);

				return action;
			}

			public Marks Mark { get; set; }
			public string[] Args { get; private set; }
		}

		public static string ToCsName(string name)
		{
			if (name == "")
				return "";

			var parts = name.Split('-');

			string result = "";
			foreach (var part in parts)
				result += part[0].ToString().ToUpper() + part.Substring(1).ToLower();
			return result;
		}
	}
}