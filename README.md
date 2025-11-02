# Coding_Lab11


#### How does each pathfinding algorithms calculate and prioritize paths?
- BFS explores nodes layer by layer from the start. It treats all moves as having equal cost. It prioritizes shortest number of steps.
- Dijikstra expands nodes with lowest total cost from the start. It works with varying terrain costs. It prioritizes lowest cumulative cost so far.
- A* combines Dijkstra's accuracy with a heuristic that estimates distance to goal. It prioritizes cost so far + estimated distance to goal.

#### What challenges arise when dynamically updating obstacles in real-time?

#### Which algorithm should you choose and how should you adapt it for larger grids or open-world settings?
A* because it’s fast, accurate, and can be optimized well. For larger grids/open-world settings, we can use Hierarchical A* which can divide map into regions and plan at multiple levels.

#### What would your approach be if you were to add weighted cells (e.g., "difficult terrain" areas)?
A* I GUESS (=_=)
