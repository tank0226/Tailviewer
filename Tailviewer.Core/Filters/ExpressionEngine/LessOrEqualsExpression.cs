﻿namespace Tailviewer.Core.Filters.ExpressionEngine
{
	internal sealed class LessOrEqualsExpression
		: BinaryNumericExpression
	{
		public LessOrEqualsExpression(IExpression lhs, IExpression rhs)
			: base(lhs, rhs)
		{}

		#region Overrides of BinaryNumericExpression

		protected override object Evaluate(long lhs, long rhs)
		{
			return lhs <= rhs;
		}

		protected override TokenType TokenType => TokenType.LessOrEquals;

		#endregion
	}
}