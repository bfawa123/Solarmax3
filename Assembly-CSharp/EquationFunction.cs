using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

public static class EquationFunction
{
	private enum TokenType
	{
		Number,
		Variable,
		Operator,
		Function
	}

	private enum Op
	{
		Add,
		Sub,
		Mul,
		Div,
		Pow,
		Neg
	}

	private struct Token
	{
		public TokenType Type;
		public float Number;
		public char Var;
		public Op Operator;
		public string Func;
	}

	private struct StackItem
	{
		public int Kind;
		public Op Op;
		public string Func;
	}

	public static bool TryCreate(string equation, out Func<float, float> func)
	{
		func = null;
		string text;
		if (!EquationFunction.TryNormalize(equation, out text))
		{
			return false;
		}
		text = EquationFunction.InsertImplicitMultiplication(text);
		List<Token> rpn;
		if (!EquationFunction.TryParseToRpn(text, out rpn))
		{
			return false;
		}
		func = ((float x) => EquationFunction.EvalRpn(rpn, x, 0f, 0f));
		return true;
	}

	public static bool TryCreateT(string expr, out Func<float, float> func)
	{
		func = null;
		if (string.IsNullOrEmpty(expr))
		{
			return false;
		}
		string text = EquationFunction.InsertImplicitMultiplication(expr.Trim());
		List<Token> rpn;
		if (!EquationFunction.TryParseToRpn(text, out rpn))
		{
			return false;
		}
		func = ((float t) => EquationFunction.EvalRpn(rpn, t, 0f, t));
		return true;
	}

	public static bool TryCreateXY(string equation, out Func<float, float, float> func)
	{
		func = null;
		if (string.IsNullOrEmpty(equation))
		{
			return false;
		}
		string expr = equation.Trim();
		int num = expr.IndexOf('=');
		if (num >= 0)
		{
			string text = expr.Substring(0, num).Trim();
			string text2 = expr.Substring(num + 1).Trim();
			expr = "(" + text + ")-(" + text2 + ")";
		}
		expr = EquationFunction.InsertImplicitMultiplication(expr);
		List<Token> rpn;
		if (!EquationFunction.TryParseToRpn(expr, out rpn))
		{
			return false;
		}
		func = ((float x, float y) => EquationFunction.EvalRpn(rpn, x, y, 0f));
		return true;
	}

	public static bool TryCreateParametric(string equationX, string equationY, out Func<float, float> fx, out Func<float, float> fy)
	{
		fx = null;
		fy = null;
		if (string.IsNullOrEmpty(equationX) || string.IsNullOrEmpty(equationY))
		{
			return false;
		}
		string text = equationX.Trim();
		string text2 = equationY.Trim();
		int num = text.IndexOf('=');
		if (num >= 0)
		{
			text = text.Substring(num + 1).Trim();
		}
		int num2 = text2.IndexOf('=');
		if (num2 >= 0)
		{
			text2 = text2.Substring(num2 + 1).Trim();
		}
		text = EquationFunction.InsertImplicitMultiplication(text);
		text2 = EquationFunction.InsertImplicitMultiplication(text2);
		List<Token> rpn;
		List<Token> rpn2;
		if (!EquationFunction.TryParseToRpn(text, out rpn) || !EquationFunction.TryParseToRpn(text2, out rpn2))
		{
			return false;
		}
		fx = ((float t) => EquationFunction.EvalRpn(rpn, t, 0f, t));
		fy = ((float t) => EquationFunction.EvalRpn(rpn2, t, 0f, t));
		return true;
	}

	private static bool TryNormalize(string equation, out string expr)
	{
		expr = null;
		if (string.IsNullOrEmpty(equation))
		{
			return false;
		}
		string text = equation.Trim();
		int num = text.IndexOf('=');
		if (num < 0)
		{
			expr = text;
			return true;
		}
		string text2 = text.Substring(0, num).Trim();
		string text3 = text.Substring(num + 1).Trim();
		if (text2.Equals("y", StringComparison.OrdinalIgnoreCase))
		{
			expr = text3;
			return true;
		}
		if (text3.Equals("y", StringComparison.OrdinalIgnoreCase))
		{
			expr = text2;
			return true;
		}
		string text4;
		if (EquationFunction.TryInvertYFunc(text2, text3, out text4) || EquationFunction.TryInvertYFunc(text3, text2, out text4))
		{
			expr = text4;
			return true;
		}
		expr = text3;
		return true;
	}

	private static string InsertImplicitMultiplication(string expr)
	{
		if (string.IsNullOrEmpty(expr))
		{
			return expr;
		}
		List<string> list = new List<string>();
		List<int> list2 = new List<int>();
		int i = 0;
		while (i < expr.Length)
		{
			char c = expr[i];
			if (char.IsWhiteSpace(c))
			{
				i++;
			}
			else if (char.IsDigit(c) || c == '.')
			{
				int num = i;
				i++;
				while (i < expr.Length && (char.IsDigit(expr[i]) || expr[i] == '.'))
				{
					i++;
				}
				list.Add(expr.Substring(num, i - num));
				list2.Add(0);
			}
			else if (char.IsLetter(c) || c == '_')
			{
				int num2 = i;
				i++;
				while (i < expr.Length && (char.IsLetter(expr[i]) || char.IsDigit(expr[i]) || expr[i] == '_'))
				{
					i++;
				}
				list.Add(expr.Substring(num2, i - num2));
				list2.Add(1);
			}
			else if (c == '(')
			{
				list.Add("(");
				list2.Add(2);
				i++;
			}
			else if (c == ')')
			{
				list.Add(")");
				list2.Add(3);
				i++;
			}
			else
			{
				list.Add(c.ToString());
				list2.Add(4);
				i++;
			}
		}
		StringBuilder stringBuilder = new StringBuilder(expr.Length + 8);
		for (int j = 0; j < list.Count; j++)
		{
			if (j > 0)
			{
				int num3 = list2[j - 1];
				int num4 = list2[j];
				string text = list[j - 1];
				string text2 = list[j];
				bool flag = num3 == 0 || num3 == 1 || num3 == 3;
				bool flag2 = num4 == 0 || num4 == 1 || num4 == 2;
				if (flag && flag2)
				{
					bool flag3 = num3 == 1 && num4 == 2 && EquationFunction.IsFunction(text);
					if (!flag3)
					{
						stringBuilder.Append('*');
					}
				}
			}
			stringBuilder.Append(list[j]);
		}
		return stringBuilder.ToString();
	}

	private static bool TryInvertYFunc(string left, string right, out string expr)
	{
		expr = null;
		if (string.IsNullOrEmpty(left))
		{
			return false;
		}
		string text = left.Replace(" ", string.Empty);
		if (!text.EndsWith("y", StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}
		string text2 = text.Substring(0, text.Length - 1);
		if (text2.Equals("sin", StringComparison.OrdinalIgnoreCase))
		{
			expr = "asin(" + right + ")";
			return true;
		}
		if (text2.Equals("cos", StringComparison.OrdinalIgnoreCase))
		{
			expr = "acos(" + right + ")";
			return true;
		}
		if (text2.Equals("tan", StringComparison.OrdinalIgnoreCase))
		{
			expr = "atan(" + right + ")";
			return true;
		}
		return false;
	}

	private static bool TryParseToRpn(string expr, out List<Token> rpn)
	{
		rpn = null;
		if (string.IsNullOrEmpty(expr))
		{
			return false;
		}
		List<Token> list = new List<Token>();
		Stack<StackItem> stack = new Stack<StackItem>();
		int i = 0;
		bool flag = true;
		while (i < expr.Length)
		{
			char c = expr[i];
			if (char.IsWhiteSpace(c))
			{
				i++;
			}
			else if (char.IsDigit(c) || c == '.')
			{
				int num = i;
				i++;
				while (i < expr.Length && (char.IsDigit(expr[i]) || expr[i] == '.'))
				{
					i++;
				}
				float number;
				if (!float.TryParse(expr.Substring(num, i - num), NumberStyles.Float, CultureInfo.InvariantCulture, out number))
				{
					return false;
				}
				list.Add(new Token
				{
					Type = TokenType.Number,
					Number = number
				});
				flag = false;
			}
			else if (char.IsLetter(c) || c == '_')
			{
				int num2 = i;
				i++;
				while (i < expr.Length && (char.IsLetter(expr[i]) || char.IsDigit(expr[i]) || expr[i] == '_'))
				{
					i++;
				}
				string text = expr.Substring(num2, i - num2);
				if (text.Equals("x", StringComparison.OrdinalIgnoreCase) || text.Equals("y", StringComparison.OrdinalIgnoreCase) || text.Equals("t", StringComparison.OrdinalIgnoreCase))
				{
					char var = char.ToLowerInvariant(text[0]);
					list.Add(new Token
					{
						Type = TokenType.Variable,
						Var = var
					});
					flag = false;
				}
				else if (EquationFunction.IsConstant(text))
				{
					list.Add(new Token
					{
						Type = TokenType.Number,
						Number = EquationFunction.GetConstant(text)
					});
					flag = false;
				}
				else if (EquationFunction.IsFunction(text))
				{
					stack.Push(new StackItem
					{
						Kind = 1,
						Func = text
					});
					flag = true;
				}
				else
				{
					return false;
				}
			}
			else if (c == '(')
			{
				stack.Push(new StackItem
				{
					Kind = 2
				});
				i++;
				flag = true;
			}
			else if (c == ')')
			{
				while (stack.Count > 0 && stack.Peek().Kind != 2)
				{
					StackItem stackItem = stack.Pop();
					if (stackItem.Kind == 0)
					{
						list.Add(new Token
						{
							Type = TokenType.Operator,
							Operator = stackItem.Op
						});
					}
					else if (stackItem.Kind == 1)
					{
						list.Add(new Token
						{
							Type = TokenType.Function,
							Func = stackItem.Func
						});
					}
				}
				if (stack.Count == 0 || stack.Peek().Kind != 2)
				{
					return false;
				}
				stack.Pop();
				if (stack.Count > 0 && stack.Peek().Kind == 1)
				{
					StackItem stackItem2 = stack.Pop();
					list.Add(new Token
					{
						Type = TokenType.Function,
						Func = stackItem2.Func
					});
				}
				i++;
				flag = false;
			}
			else
			{
				if (flag && c == '+')
				{
					i++;
					continue;
				}
				Op op;
				if (!EquationFunction.TryReadOperator(c, flag, out op))
				{
					return false;
				}
				while (stack.Count > 0 && stack.Peek().Kind == 0)
				{
					Op op2 = stack.Peek().Op;
					int num3 = EquationFunction.Precedence(op);
					int num4 = EquationFunction.Precedence(op2);
					bool flag2 = EquationFunction.IsRightAssociative(op);
					if ((!flag2 && num3 <= num4) || (flag2 && num3 < num4))
					{
						StackItem stackItem3 = stack.Pop();
						list.Add(new Token
						{
							Type = TokenType.Operator,
							Operator = stackItem3.Op
						});
					}
					else
					{
						break;
					}
				}
				stack.Push(new StackItem
				{
					Kind = 0,
					Op = op
				});
				i++;
				flag = true;
			}
		}
		while (stack.Count > 0)
		{
			StackItem stackItem4 = stack.Pop();
			if (stackItem4.Kind == 2)
			{
				return false;
			}
			if (stackItem4.Kind == 0)
			{
				list.Add(new Token
				{
					Type = TokenType.Operator,
					Operator = stackItem4.Op
				});
			}
			else if (stackItem4.Kind == 1)
			{
				list.Add(new Token
				{
					Type = TokenType.Function,
					Func = stackItem4.Func
				});
			}
		}
		rpn = list;
		return true;
	}

	private static bool TryReadOperator(char c, bool expectUnary, out Op op)
	{
		op = Op.Add;
		if (c == '+')
		{
			op = Op.Add;
			return true;
		}
		if (c == '-')
		{
			op = (expectUnary ? Op.Neg : Op.Sub);
			return true;
		}
		if (c == '*')
		{
			op = Op.Mul;
			return true;
		}
		if (c == '/')
		{
			op = Op.Div;
			return true;
		}
		if (c == '^')
		{
			op = Op.Pow;
			return true;
		}
		return false;
	}

	private static int Precedence(Op op)
	{
		switch (op)
		{
		case Op.Add:
		case Op.Sub:
			return 1;
		case Op.Mul:
		case Op.Div:
			return 2;
		case Op.Pow:
			return 3;
		case Op.Neg:
			return 4;
		default:
			return 0;
		}
	}

	private static bool IsRightAssociative(Op op)
	{
		return op == Op.Pow || op == Op.Neg;
	}

	private static bool IsFunction(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return false;
		}
		string text = s.ToLowerInvariant();
		return text == "sin" || text == "cos" || text == "tan" || text == "asin" || text == "acos" || text == "atan" || text == "sqrt" || text == "abs" || text == "log" || text == "ln" || text == "exp";
	}

	private static bool IsConstant(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return false;
		}
		string text = s.ToLowerInvariant();
		return text == "pi" || text == "e";
	}

	private static float GetConstant(string s)
	{
		string text = s.ToLowerInvariant();
		if (text == "pi")
		{
			return 3.1415926f;
		}
		if (text == "e")
		{
			return 2.7182817f;
		}
		return 0f;
	}

	private static float EvalRpn(List<Token> rpn, float x, float y, float t)
	{
		Stack<float> stack = new Stack<float>();
		for (int i = 0; i < rpn.Count; i++)
		{
			Token token = rpn[i];
			if (token.Type == TokenType.Number)
			{
				stack.Push(token.Number);
			}
			else if (token.Type == TokenType.Variable)
			{
				if (token.Var == 'x')
				{
					stack.Push(x);
				}
				else if (token.Var == 'y')
				{
					stack.Push(y);
				}
				else if (token.Var == 't')
				{
					stack.Push(t);
				}
				else
				{
					stack.Push(0f);
				}
			}
			else if (token.Type == TokenType.Operator)
			{
				if (token.Operator == Op.Neg)
				{
					if (stack.Count < 1)
					{
						return 0f;
					}
					float num = stack.Pop();
					stack.Push(-num);
				}
				else
				{
					if (stack.Count < 2)
					{
						return 0f;
					}
					float b = stack.Pop();
					float a = stack.Pop();
					switch (token.Operator)
					{
					case Op.Add:
						stack.Push(a + b);
						break;
					case Op.Sub:
						stack.Push(a - b);
						break;
					case Op.Mul:
						stack.Push(a * b);
						break;
					case Op.Div:
						stack.Push(a / b);
						break;
					case Op.Pow:
						stack.Push(Mathf.Pow(a, b));
						break;
					}
				}
			}
			else if (token.Type == TokenType.Function)
			{
				if (EquationFunction.IsConstant(token.Func))
				{
					stack.Push(EquationFunction.GetConstant(token.Func));
				}
				else
				{
					if (stack.Count < 1)
					{
						return 0f;
					}
					float num2 = stack.Pop();
					stack.Push(EquationFunction.EvalFunc(token.Func, num2));
				}
			}
		}
		if (stack.Count != 1)
		{
			return 0f;
		}
		float num3 = stack.Pop();
		if (float.IsNaN(num3) || float.IsInfinity(num3))
		{
			return 0f;
		}
		return num3;
	}

	private static float EvalFunc(string func, float v)
	{
		string text = func.ToLowerInvariant();
		if (text == "sin")
		{
			return Mathf.Sin(v);
		}
		if (text == "cos")
		{
			return Mathf.Cos(v);
		}
		if (text == "tan")
		{
			return Mathf.Tan(v);
		}
		if (text == "asin")
		{
			return Mathf.Asin(v);
		}
		if (text == "acos")
		{
			return Mathf.Acos(v);
		}
		if (text == "atan")
		{
			return Mathf.Atan(v);
		}
		if (text == "sqrt")
		{
			return Mathf.Sqrt(v);
		}
		if (text == "abs")
		{
			return Mathf.Abs(v);
		}
		if (text == "log")
		{
			return Mathf.Log10(v);
		}
		if (text == "ln")
		{
			return Mathf.Log(v);
		}
		if (text == "exp")
		{
			return Mathf.Exp(v);
		}
		return 0f;
	}

	private struct Segment
	{
		public Vector2 A;
		public Vector2 B;
	}

	public static bool TrySampleImplicit(string equation, string orbitBox, float hintX, float hintY, out List<Vector3> points)
	{
		points = null;
		Func<float, float, float> func;
		if (!EquationFunction.TryCreateXY(equation, out func))
		{
			return false;
		}
		float xmin;
		float xmax;
		float ymin;
		float ymax;
		if (!EquationFunction.TryParseBox(orbitBox, out xmin, out xmax, out ymin, out ymax))
		{
			return false;
		}
		float num = xmax - xmin;
		float num2 = ymax - ymin;
		int num3 = Mathf.Clamp(Mathf.RoundToInt(num / 0.1f), 80, 240);
		int num4 = Mathf.Clamp(Mathf.RoundToInt(num2 / 0.1f), 80, 240);
		float num5 = num / (float)num3;
		float num6 = num2 / (float)num4;
		float[,] array = new float[num3 + 1, num4 + 1];
		for (int i = 0; i <= num3; i++)
		{
			float x = xmin + (float)i * num5;
			for (int j = 0; j <= num4; j++)
			{
				float y = ymin + (float)j * num6;
				float num7 = func(x, y);
				if (float.IsNaN(num7) || float.IsInfinity(num7))
				{
					num7 = 0f;
				}
				array[i, j] = num7;
			}
		}
		List<Segment> list = new List<Segment>();
		for (int k = 0; k < num3; k++)
		{
			float x2 = xmin + (float)k * num5;
			float x3 = x2 + num5;
			for (int l = 0; l < num4; l++)
			{
				float y2 = ymin + (float)l * num6;
				float y3 = y2 + num6;
				float v = array[k, l];
				float v2 = array[k + 1, l];
				float v3 = array[k + 1, l + 1];
				float v4 = array[k, l + 1];
				bool flag = v > 0f;
				bool flag2 = v2 > 0f;
				bool flag3 = v3 > 0f;
				bool flag4 = v4 > 0f;
				int num7 = (flag ? 1 : 0) | (flag2 ? 2 : 0) | (flag3 ? 4 : 0) | (flag4 ? 8 : 0);
				if (num7 == 0 || num7 == 15)
				{
					continue;
				}
				Vector2 vector = new Vector2(x2, y2);
				Vector2 vector2 = new Vector2(x3, y2);
				Vector2 vector3 = new Vector2(x3, y3);
				Vector2 vector4 = new Vector2(x2, y3);
				Vector2 vector5 = Vector2.zero;
				Vector2 vector6 = Vector2.zero;
				Vector2 vector7 = Vector2.zero;
				Vector2 vector8 = Vector2.zero;
				bool flag5 = flag != flag2;
				bool flag6 = flag2 != flag3;
				bool flag7 = flag3 != flag4;
				bool flag8 = flag4 != flag;
				if (flag5)
				{
					vector5 = EquationFunction.LerpIso(vector, vector2, v, v2);
				}
				if (flag6)
				{
					vector6 = EquationFunction.LerpIso(vector2, vector3, v2, v3);
				}
				if (flag7)
				{
					vector7 = EquationFunction.LerpIso(vector3, vector4, v3, v4);
				}
				if (flag8)
				{
					vector8 = EquationFunction.LerpIso(vector4, vector, v4, v);
				}
				float center = func((x2 + x3) * 0.5f, (y2 + y3) * 0.5f);
				EquationFunction.AddMarchingSegments(num7, center > 0f, flag5, flag6, flag7, flag8, vector5, vector6, vector7, vector8, list);
			}
		}
		if (list.Count == 0)
		{
			return false;
		}
		List<Vector2> list2;
		float quantize = Mathf.Clamp(Mathf.Min(num5, num6) * 0.25f, 0.002f, 0.05f);
		if (!EquationFunction.TryBuildBestPolyline(list, hintX, hintY, quantize, out list2))
		{
			return false;
		}
		points = new List<Vector3>(list2.Count);
		for (int m = 0; m < list2.Count; m++)
		{
			Vector2 vector9 = list2[m];
			points.Add(new Vector3(vector9.x, vector9.y, 0f));
		}
		return points.Count >= 2;
	}

	private static bool TryParseBox(string orbitBox, out float xmin, out float xmax, out float ymin, out float ymax)
	{
		xmin = 0f;
		xmax = 0f;
		ymin = 0f;
		ymax = 0f;
		if (string.IsNullOrEmpty(orbitBox))
		{
			return false;
		}
		string[] array = orbitBox.Split(new char[]
		{
			','
		});
		if (array == null || array.Length < 4)
		{
			return false;
		}
		if (!float.TryParse(array[0], NumberStyles.Float, CultureInfo.InvariantCulture, out xmin) || !float.TryParse(array[1], NumberStyles.Float, CultureInfo.InvariantCulture, out xmax) || !float.TryParse(array[2], NumberStyles.Float, CultureInfo.InvariantCulture, out ymin) || !float.TryParse(array[3], NumberStyles.Float, CultureInfo.InvariantCulture, out ymax))
		{
			return false;
		}
		if (xmin > xmax)
		{
			float num = xmin;
			xmin = xmax;
			xmax = num;
		}
		if (ymin > ymax)
		{
			float num2 = ymin;
			ymin = ymax;
			ymax = num2;
		}
		return true;
	}

	private static Vector2 LerpIso(Vector2 a, Vector2 b, float va, float vb)
	{
		float num = va - vb;
		if (Mathf.Abs(num) < 0.000001f)
		{
			return (a + b) * 0.5f;
		}
		float t = va / num;
		return Vector2.Lerp(a, b, t);
	}

	private static void AddMarchingSegments(int idx, bool centerPositive, bool e0, bool e1, bool e2, bool e3, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, List<Segment> segs)
	{
		switch (idx)
		{
		case 1:
		case 14:
			EquationFunction.AddSeg(segs, p3, p0);
			return;
		case 2:
		case 13:
			EquationFunction.AddSeg(segs, p0, p1);
			return;
		case 3:
		case 12:
			EquationFunction.AddSeg(segs, p3, p1);
			return;
		case 4:
		case 11:
			EquationFunction.AddSeg(segs, p1, p2);
			return;
		case 6:
		case 9:
			EquationFunction.AddSeg(segs, p0, p2);
			return;
		case 7:
		case 8:
			EquationFunction.AddSeg(segs, p3, p2);
			return;
		case 5:
		{
			if (centerPositive)
			{
				EquationFunction.AddSeg(segs, p3, p0);
				EquationFunction.AddSeg(segs, p2, p1);
				return;
			}
			EquationFunction.AddSeg(segs, p3, p2);
			EquationFunction.AddSeg(segs, p0, p1);
			return;
		}
		case 10:
		{
			if (centerPositive)
			{
				EquationFunction.AddSeg(segs, p0, p1);
				EquationFunction.AddSeg(segs, p2, p3);
				return;
			}
			EquationFunction.AddSeg(segs, p0, p3);
			EquationFunction.AddSeg(segs, p1, p2);
			return;
		}
		default:
			return;
		}
	}

	private static void AddSeg(List<Segment> segs, Vector2 a, Vector2 b)
	{
		Segment item = new Segment();
		item.A = a;
		item.B = b;
		segs.Add(item);
	}

	private static bool TryBuildBestPolyline(List<Segment> segs, float hintX, float hintY, float quantize, out List<Vector2> polyline)
	{
		polyline = null;
		Dictionary<long, int> dictionary = new Dictionary<long, int>();
		List<Vector2> list = new List<Vector2>();
		List<List<int>> list2 = new List<List<int>>();
		Dictionary<long, int> dictionary2 = new Dictionary<long, int>();
		for (int i = 0; i < segs.Count; i++)
		{
			Segment segment = segs[i];
			int num = EquationFunction.GetVertIndex(segment.A, quantize, dictionary, list, list2);
			int num2 = EquationFunction.GetVertIndex(segment.B, quantize, dictionary, list, list2);
			if (num == num2)
			{
				continue;
			}
			list2[num].Add(num2);
			list2[num2].Add(num);
			long key = EquationFunction.EdgeKey(num, num2);
			int num3;
			dictionary2.TryGetValue(key, out num3);
			dictionary2[key] = num3 + 1;
		}
		int count = list.Count;
		int[] array = new int[count];
		for (int j = 0; j < count; j++)
		{
			array[j] = -1;
		}
		int num4 = 0;
		float num5 = float.MaxValue;
		int num6 = -1;
		for (int k = 0; k < count; k++)
		{
			if (array[k] >= 0)
			{
				continue;
			}
			Stack<int> stack = new Stack<int>();
			stack.Push(k);
			array[k] = num4;
			float num7 = float.MaxValue;
			int num8 = k;
			while (stack.Count > 0)
			{
				int num9 = stack.Pop();
				Vector2 vector = list[num9];
				float num10 = (vector.x - hintX) * (vector.x - hintX) + (vector.y - hintY) * (vector.y - hintY);
				if (num10 < num7)
				{
					num7 = num10;
					num8 = num9;
				}
				List<int> list3 = list2[num9];
				for (int l = 0; l < list3.Count; l++)
				{
					int num11 = list3[l];
					if (array[num11] < 0)
					{
						array[num11] = num4;
						stack.Push(num11);
					}
				}
			}
			if (num7 < num5)
			{
				num5 = num7;
				num6 = num4;
			}
			num4++;
		}
		if (num6 < 0)
		{
			return false;
		}
		int[] array2 = new int[count];
		for (int m = 0; m < count; m++)
		{
			array2[m] = 0;
		}
		foreach (KeyValuePair<long, int> keyValuePair in dictionary2)
		{
			long key2 = keyValuePair.Key;
			int num12 = (int)(key2 >> 32);
			int num13 = (int)(key2 & 4294967295L);
			int value = keyValuePair.Value;
			if (num12 >= 0 && num12 < count && num13 >= 0 && num13 < count && array[num12] == num6 && array[num13] == num6)
			{
				array2[num12] += value;
				array2[num13] += value;
			}
		}
		int num14 = -1;
		float num15 = float.MaxValue;
		int num16 = -1;
		float num17 = float.MaxValue;
		for (int n = 0; n < count; n++)
		{
			if (array[n] == num6 && (array2[n] & 1) == 1)
			{
				Vector2 vector2 = list[n];
				float num18 = (vector2.x - hintX) * (vector2.x - hintX) + (vector2.y - hintY) * (vector2.y - hintY);
				if (num18 < num15)
				{
					num17 = num15;
					num16 = num14;
					num15 = num18;
					num14 = n;
				}
				else if (num18 < num17)
				{
					num17 = num18;
					num16 = n;
				}
			}
		}
		int num19 = -1;
		float num20 = float.MaxValue;
		for (int num21 = 0; num21 < count; num21++)
		{
			if (array[num21] == num6)
			{
				Vector2 vector3 = list[num21];
				float num22 = (vector3.x - hintX) * (vector3.x - hintX) + (vector3.y - hintY) * (vector3.y - hintY);
				if (num22 < num20)
				{
					num20 = num22;
					num19 = num21;
				}
			}
		}
		int start = (num14 >= 0) ? num14 : num19;
		Dictionary<long, int> dictionary3 = new Dictionary<long, int>();
		foreach (KeyValuePair<long, int> keyValuePair2 in dictionary2)
		{
			long key3 = keyValuePair2.Key;
			int num23 = (int)(key3 >> 32);
			int num24 = (int)(key3 & 4294967295L);
			if (num23 >= 0 && num23 < count && num24 >= 0 && num24 < count && array[num23] == num6 && array[num24] == num6)
			{
				dictionary3[key3] = keyValuePair2.Value;
			}
		}
		if (dictionary3.Count == 0)
		{
			return false;
		}
		Stack<int> stack2 = new Stack<int>();
		List<int> list4 = new List<int>();
		stack2.Push(start);
		while (stack2.Count > 0)
		{
			int num26 = stack2.Peek();
			List<int> list5 = list2[num26];
			int num28 = -1;
			for (int num27 = 0; num27 < list5.Count; num27++)
			{
				int num29 = list5[num27];
				long key4 = EquationFunction.EdgeKey(num26, num29);
				int num30;
				if (dictionary3.TryGetValue(key4, out num30) && num30 > 0)
				{
					num28 = num29;
					break;
				}
			}
			if (num28 >= 0)
			{
				long key5 = EquationFunction.EdgeKey(num26, num28);
				dictionary3[key5] = dictionary3[key5] - 1;
				stack2.Push(num28);
			}
			else
			{
				stack2.Pop();
				list4.Add(num26);
			}
		}
		if (list4.Count < 2)
		{
			return false;
		}
		List<Vector2> list6 = new List<Vector2>(list4.Count);
		Vector2 vector4 = Vector2.zero;
		bool flag = false;
		for (int num31 = list4.Count - 1; num31 >= 0; num31--)
		{
			Vector2 vector5 = list[list4[num31]];
			if (!flag)
			{
				list6.Add(vector5);
				vector4 = vector5;
				flag = true;
			}
			else if ((vector5 - vector4).sqrMagnitude > 0.0000001f)
			{
				list6.Add(vector5);
				vector4 = vector5;
			}
		}
		if (list6.Count >= 2 && (list6[0] - list6[list6.Count - 1]).sqrMagnitude <= quantize * quantize * 4f)
		{
			list6.Add(list6[0]);
		}
		if (list6.Count < 6)
		{
			return EquationFunction.TryBuildBestPolylineGreedy(list, list2, hintX, hintY, out polyline);
		}
		polyline = list6;
		return true;
	}
	
	private static bool TryBuildBestPolylineGreedy(List<Vector2> verts, List<List<int>> adj, float hintX, float hintY, out List<Vector2> polyline)
	{
		polyline = null;
		HashSet<long> hashSet = new HashSet<long>();
		List<Vector2> list = null;
		float num = float.MaxValue;
		for (int i = 0; i < verts.Count; i++)
		{
			List<int> list2 = adj[i];
			for (int j = 0; j < list2.Count; j++)
			{
				int num2 = list2[j];
				long edgeKey = EquationFunction.EdgeKey(i, num2);
				if (hashSet.Contains(edgeKey))
				{
					continue;
				}
				List<int> list3 = new List<int>();
				int num3 = -1;
				int num4 = i;
				int num5 = num2;
				list3.Add(num4);
				int num6 = 0;
				while (num6 < 10000)
				{
					long edgeKey2 = EquationFunction.EdgeKey(num4, num5);
					hashSet.Add(edgeKey2);
					num3 = num4;
					num4 = num5;
					list3.Add(num4);
					List<int> list4 = adj[num4];
					int num7 = -1;
					float num8 = -2f;
					Vector2 vector = (num3 >= 0) ? (verts[num4] - verts[num3]) : Vector2.right;
					if (vector.sqrMagnitude > 0.000001f)
					{
						vector.Normalize();
					}
					for (int k = 0; k < list4.Count; k++)
					{
						int neighbor = list4[k];
						if (neighbor != num3)
						{
							long edgeKey3 = EquationFunction.EdgeKey(num4, neighbor);
							if (!hashSet.Contains(edgeKey3))
							{
								Vector2 vector2 = verts[neighbor] - verts[num4];
								if (vector2.sqrMagnitude <= 0.000001f)
								{
									continue;
								}
								vector2.Normalize();
								float num9 = Vector2.Dot(vector, vector2);
								if (num9 > num8)
								{
									num8 = num9;
									num7 = neighbor;
								}
							}
						}
					}
					if (num7 < 0)
					{
						break;
					}
					num5 = num7;
					if (num5 == i)
					{
						break;
					}
					num6++;
				}
				if (list3.Count < 6)
				{
					continue;
				}
				float num10 = float.MaxValue;
				for (int l = 0; l < list3.Count; l++)
				{
					Vector2 vector3 = verts[list3[l]];
					float num11 = (vector3.x - hintX) * (vector3.x - hintX) + (vector3.y - hintY) * (vector3.y - hintY);
					if (num11 < num10)
					{
						num10 = num11;
					}
				}
				if (num10 < num)
				{
					num = num10;
					list = new List<Vector2>(list3.Count);
					for (int m = 0; m < list3.Count; m++)
					{
						list.Add(verts[list3[m]]);
					}
				}
			}
		}
		if (list == null)
		{
			return false;
		}
		polyline = list;
		return true;
	}

	private static int GetVertIndex(Vector2 p, float quantize, Dictionary<long, int> dict, List<Vector2> verts, List<List<int>> adj)
	{
		float num = (quantize > 0.000001f) ? (1f / quantize) : 1000f;
		int num2 = Mathf.RoundToInt(p.x * num);
		int num3 = Mathf.RoundToInt(p.y * num);
		long key = ((long)num2 << 32) ^ (uint)num3;
		int result;
		if (dict.TryGetValue(key, out result))
		{
			return result;
		}
		result = verts.Count;
		dict[key] = result;
		verts.Add(p);
		adj.Add(new List<int>());
		return result;
	}

	private static long EdgeKey(int a, int b)
	{
		int num = (a < b) ? a : b;
		int num2 = (a < b) ? b : a;
		return ((long)num << 32) ^ (uint)num2;
	}
}
