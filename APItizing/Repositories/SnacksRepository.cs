using APItizing.Models;

namespace APItizing.Repositories
{
    /// <summary>
    /// Basic in-memory repository for Snacks
    /// </summary>
    public sealed class SnacksRepository
    {
        private static readonly List<Snack> _snacks = [
            new() {
                Id = 1,
                Name = "Apple Slices with Peanut Butter",
                Calories = 200
            },
            new() {
                Id = 2,
                Name = "Greek Yogurt Parfait",
                Calories = 250
            },
            new() {
                Id = 3,
                Name = "Mixed Nuts",
                Calories = 180
            },
            new() {
                Id = 4,
                Name = "Hummus and Veggie Sticks",
                Calories = 125
            },
            new() {
                Id = 5,
                Name = "Dark Chocolate",
                Calories = 175
            }
        ];

        /// <summary>
        /// Get a list of Snacks based on the filters
        /// </summary>
        /// <param name="snackFilters"></param>
        /// <returns>List of Snacks</returns>
        public IEnumerable<Snack> Browse(SnackFilters snackFilters)
        {
            var query = _snacks.AsEnumerable();

            if (snackFilters.MaxCalories != null)
            {
                query = query.Where(s => s.Calories <= snackFilters.MaxCalories);
            }

            return query.OrderBy(s => s.Id);
        }

        /// <summary>
        /// Get a single Snack by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A single Snack</returns>
        public Snack? Read(int id)
        {
            return _snacks.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Create a snack based on the Snack parameter
        /// </summary>
        /// <param name="snack"></param>
        /// <returns>The created Snack</returns>
        public Snack Create(Snack snack)
        {
            snack.Id = _snacks.Max(s => s.Id) + 1;

            _snacks.Add(snack);

            return snack;
        }

        /// <summary>
        /// Update a snack based on the Snack parameter
        /// </summary>
        /// <param name="id"></param>
        /// <param name="snack"></param>
        /// <returns>The updated Snack</returns>
        public Snack Update(int id, Snack snack)
        {
            _snacks.Remove(_snacks.First(s => s.Id == id));

            _snacks.Add(snack);

            return snack;
        }

        /// <summary>
        /// Delete a snack based on the id parameter
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            _snacks.Remove(_snacks.First(s => s.Id == id));
        }
    }
}
