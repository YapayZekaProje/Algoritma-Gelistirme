import heapq

class Node:
    def __init__(self, x, y, walkable=True, cost=1):
        self.x = x
        self.y = y
        self.walkable = walkable
        self.cost = cost  # Cost to travel through this node
        self.g_cost = float('inf')  # Cost from start to node
        self.h_cost = 0  # Heuristic cost to the target
        self.f_cost = float('inf')  # Total cost (g + h)
        self.parent = None

    def __lt__(self, other):
        return self.f_cost < other.f_cost or (self.f_cost == other.f_cost and self.h_cost < other.h_cost)


class Grid:
    def __init__(self, width, height):
        self.width = width
        self.height = height
        self.grid = [[Node(x, y) for y in range(height)] for x in range(width)]

    def get_neighbours(self, node):
        neighbours = []
        for dx, dy in [(-1, 0), (1, 0), (0, -1), (0, 1)]:
            nx, ny = node.x + dx, node.y + dy
            if 0 <= nx < self.width and 0 <= ny < self.height:
                neighbours.append(self.grid[nx][ny])
        return neighbours

    def node_from_world_point(self, x, y):
        return self.grid[x][y]


def get_distance(node_a, node_b):
    # Manhattan distance heuristic
    return abs(node_a.x - node_b.x) + abs(node_a.y - node_b.y)


def retrace_path(start_node, end_node):
    path = []
    current_node = end_node
    while current_node != start_node:
        path.append((current_node.x, current_node.y))
        current_node = current_node.parent
    path.append((start_node.x, start_node.y))
    path.reverse()
    return path


def a_star_pathfinding(grid, start_pos, target_pos):
    start_node = grid.node_from_world_point(*start_pos)
    target_node = grid.node_from_world_point(*target_pos)

    open_set = []
    closed_set = set()

    start_node.g_cost = 0
    start_node.h_cost = get_distance(start_node, target_node)
    start_node.f_cost = start_node.g_cost + start_node.h_cost

    heapq.heappush(open_set, start_node)

    while open_set:
        current_node = heapq.heappop(open_set)
        closed_set.add(current_node)

        # If we reach the target, retrace the path
        if current_node == target_node:
            return retrace_path(start_node, target_node), current_node.g_cost  # Return the cost as well

        for neighbour in grid.get_neighbours(current_node):
            if not neighbour.walkable or neighbour in closed_set:
                continue

            # Calculate the g_cost with the neighbour's cost
            tentative_g_cost = current_node.g_cost + neighbour.cost  # Add the cost of the current node

            if tentative_g_cost < neighbour.g_cost:
                neighbour.parent = current_node
                neighbour.g_cost = tentative_g_cost
                neighbour.h_cost = get_distance(neighbour, target_node)
                neighbour.f_cost = neighbour.g_cost + neighbour.h_cost

                if neighbour not in open_set:
                    heapq.heappush(open_set, neighbour)

    return None, None  # No path found


# Example usage:
grid = Grid(10, 10)  # A 10x10 grid
start_pos = (0, 0)  # Start position (x, y)
target_pos = (7, 7)  # Target position (x, y)

# Set some nodes as non-walkable
grid.grid[5][5].walkable = False
grid.grid[5][6].walkable = False
grid.grid[6][5].walkable = False


grid.grid[1][1].cost = 3  # Higher cost for this node
grid.grid[2][1].cost = 2  # A medium-cost node

path, path_cost = a_star_pathfinding(grid, start_pos, target_pos)
if path:
    print("Path found:", path)
    print("Total Yol Maliyeti (Path Cost):", path_cost)
else:
    print("No path found.")
