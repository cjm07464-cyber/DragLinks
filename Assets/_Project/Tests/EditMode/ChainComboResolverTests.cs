using DragLinks.Character;
using DragLinks.Linking;
using System.Collections.Generic;
using NUnit.Framework;

namespace DragLinks.Tests.EditMode
{
    public sealed class ChainComboResolverTests
    {
        [Test]
        public void LinkingTotalLineCountRegistersAsPendingTriggers()
        {
            var state = new ChainComboRuntimeState(2);
            var linking = new LinkingResolutionResult(
                new List<LinkingWaveResult>(), 3, false);

            new ChainComboResolver().RegisterLinkingResult(state, linking);

            Assert.That(state.CurrentStack, Is.EqualTo(2));
            Assert.That(state.PendingComboTriggers, Is.EqualTo(3));
        }

        [TestCase(0, 1, 1)]
        [TestCase(1, 1, 2)]
        public void SinglePendingAdvancesPersistentStack(int initialStack, int pending, int expectedStack)
        {
            var state = new ChainComboRuntimeState(initialStack, pending);
            var resolver = new ChainComboResolver();

            Assert.That(resolver.TryResolveNextStep(state, out var step), Is.True);
            Assert.That(step.ActivatedStack, Is.EqualTo(expectedStack));
            Assert.That(state.CurrentStack, Is.EqualTo(expectedStack));
            Assert.That(state.PendingComboTriggers, Is.Zero);
        }

        [Test]
        public void TwoPendingFromStackTwoActivateThreeThenFour()
        {
            var state = new ChainComboRuntimeState(2, 2);
            var steps = ResolveAll(state);

            Assert.That(steps, Is.EqualTo(new[] { 3, 4 }));
            Assert.That(state.CurrentStack, Is.EqualTo(4));
            Assert.That(state.PendingComboTriggers, Is.Zero);
        }

        [Test]
        public void FiveStackResetsCurrentStackToZero()
        {
            var state = new ChainComboRuntimeState(4, 1);
            var resolver = new ChainComboResolver();

            resolver.TryResolveNextStep(state, out var step);

            Assert.That(step.ActivatedStack, Is.EqualTo(5));
            Assert.That(step.IsFiveStack, Is.True);
            Assert.That(step.CurrentStackAfterStep, Is.Zero);
            Assert.That(state.CurrentStack, Is.Zero);
        }

        [Test]
        public void PendingAfterFiveStackContinuesFromStackOne()
        {
            var state = new ChainComboRuntimeState(4, 2);

            var steps = ResolveAll(state);

            Assert.That(steps, Is.EqualTo(new[] { 5, 1 }));
            Assert.That(state.CurrentStack, Is.EqualTo(1));
            Assert.That(state.PendingComboTriggers, Is.Zero);
        }

        [Test]
        public void SevenPendingCrossingFiveStackKeepsCorrectOrder()
        {
            var state = new ChainComboRuntimeState(3, 7);

            var steps = ResolveAll(state);

            Assert.That(steps, Is.EqualTo(new[] { 4, 5, 1, 2, 3, 4, 5 }));
            Assert.That(state.CurrentStack, Is.Zero);
            Assert.That(state.PendingComboTriggers, Is.Zero);
        }

        [Test]
        public void FiveStackCanPauseAndResumeOneStepAtATime()
        {
            var state = new ChainComboRuntimeState(4, 2);
            var resolver = new ChainComboResolver();

            resolver.TryResolveNextStep(state, out var fiveStack);
            Assert.That(fiveStack.IsFiveStack, Is.True);
            Assert.That(state.CurrentStack, Is.Zero);
            Assert.That(state.PendingComboTriggers, Is.EqualTo(1));

            // External orchestrator can perform 5-stack board work before explicitly resuming here.
            resolver.TryResolveNextStep(state, out var resumed);
            Assert.That(resumed.ActivatedStack, Is.EqualTo(1));
            Assert.That(state.CurrentStack, Is.EqualTo(1));
            Assert.That(state.PendingComboTriggers, Is.Zero);
        }

        [Test]
        public void NoPendingDoesNotChangeStack()
        {
            var state = new ChainComboRuntimeState(3);

            Assert.That(new ChainComboResolver().TryResolveNextStep(state, out var step), Is.False);
            Assert.That(step, Is.Null);
            Assert.That(state.CurrentStack, Is.EqualTo(3));
        }

        private static int[] ResolveAll(ChainComboRuntimeState state)
        {
            var resolver = new ChainComboResolver();
            var steps = new System.Collections.Generic.List<int>();
            while (resolver.TryResolveNextStep(state, out var step)) steps.Add(step.ActivatedStack);
            return steps.ToArray();
        }
    }
}
