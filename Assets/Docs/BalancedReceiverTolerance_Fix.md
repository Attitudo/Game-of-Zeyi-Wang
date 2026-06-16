# Balanced Receiver Tolerance Fix

Fix:
1. Receiver tolerance is adjusted from 0.28 to 0.65.
2. This avoids the receiver being too hard to hit because of small mirror rotation steps.
3. It is still much smaller than the previous 1.85 range, so the door should not open when the beam only passes far beside the receiver.
4. Recommended gameplay result:
   - Slightly off-center beam can still count.
   - Clearly missing the receiver should not count.
