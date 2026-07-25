# Navigation Foundation System — Hero of Eternia

This manual defines the walkable path calculation rules, slope threshold checks, and headless cell-grid matrices.

---

## 1. Walkability Criteria

To verify pathing without incurring the runtime overhead of 3D mesh baking, walkability checks evaluate cell boundaries:

- **Slope Tilt Angles:** Slope gradients are computed from neighbor height deltas:
  $$\frac{dz}{dx} = H_{\text{East}} - H_{\text{Center}}$$
  $$\frac{dz}{dy} = H_{\text{North}} - H_{\text{Center}}$$
  $$\theta = \arctan\left(\sqrt{\left(\frac{dz}{dx}\right)^2 + \left(\frac{dz}{dy}\right)^2}\right) \times \frac{180}{\pi}$$
  Cells exceeding 30 degrees angle are flagged non-walkable.
- **Water Boundaries:** Areas below `-2.5` units Y elevation are restricted.

---

## 2. Cell Grid Matrices

Each chunk generates a $16 \times 16$ boolean walkability matrix:
- **Fast Lookup:** O(1) indices lookups provide fast queries for AI pathing.
- **Multiplayer Ready:** Compact binary grid formats allow syncing chunk walkability tables over networks.
- **Headless-Safe:** Executes without requiring graphical displays or rendering server loops.
