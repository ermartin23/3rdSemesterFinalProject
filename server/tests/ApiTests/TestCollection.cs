// Disable test parallelization globally for this project.
// This prevents multiple tests from using / deleting the same Testcontainers database at once.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace tests.ApiTests;

