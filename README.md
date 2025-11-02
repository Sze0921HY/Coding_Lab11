# Coding_Lab11


#### How does each pathfinding algorithms calculate and prioritize paths?
- BFS explores nodes layer by layer from the start. It treats all moves as having equal cost. It prioritizes shortest number of steps.
- Dijikstra expands nodes with lowest total cost from the start. It works with varying terrain costs. It prioritizes lowest cumulative cost so far.
- A* combines Dijkstra's accuracy with a heuristic that estimates distance to goal. It prioritizes cost so far + estimated distance to goal.

What challenges arise when dynamically updating obstacles in real-time?

Which algorithm should you choose and how should you adapt it for larger grids or open-world settings?
A*
What would your approach be if you were to add weighted cells (e.g., "difficult terrain" areas)?
