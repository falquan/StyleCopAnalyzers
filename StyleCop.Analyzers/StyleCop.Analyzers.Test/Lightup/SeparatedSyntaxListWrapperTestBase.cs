﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public abstract class SeparatedSyntaxListWrapperTestBase
    {
        [Fact]
        public void TestBasicProperties()
        {
            var list = this.CreateList();
            Assert.Equal(0, list.Count);
            Assert.Equal(0, list.SeparatorCount);
            Assert.Equal(default(SeparatedSyntaxList<SyntaxNode>).FullSpan, list.FullSpan);
            Assert.Equal(default(SeparatedSyntaxList<SyntaxNode>).Span, list.Span);
            Assert.Equal(default(SeparatedSyntaxList<SyntaxNode>).ToString(), list.ToString());
            Assert.Equal(default(SeparatedSyntaxList<SyntaxNode>).ToFullString(), list.ToFullString());
            Assert.ThrowsAny<ArgumentException>(() => list[0]);

            if (list.UnderlyingList != null)
            {
                Assert.IsAssignableFrom(typeof(SeparatedSyntaxList<SyntaxNode>), list.UnderlyingList);
                var underlyingList = (SeparatedSyntaxList<SyntaxNode>)list.UnderlyingList;
                Assert.Equal(0, list.Count);
            }
        }

        [Fact]
        public void TestElements()
        {
            var list = this.CreateList();
            Assert.False(list.Any());
            Assert.Null(list.FirstOrDefault());
            Assert.Null(list.LastOrDefault());
            Assert.ThrowsAny<ArgumentOutOfRangeException>(() => list.First());
            Assert.ThrowsAny<ArgumentOutOfRangeException>(() => list.Last());

            if (this.TryCreateNonEmptyList(out list))
            {
                Assert.True(list.Any());
                Assert.NotNull(list.First());
                Assert.NotNull(list.FirstOrDefault());
                Assert.NotNull(list.Last());
                Assert.NotNull(list.LastOrDefault());
            }
        }

        internal abstract SeparatedSyntaxListWrapper<SyntaxNode> CreateList();

        internal abstract bool TryCreateNonEmptyList(out SeparatedSyntaxListWrapper<SyntaxNode> list);
    }
}
