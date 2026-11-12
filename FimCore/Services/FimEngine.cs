using FimCore.Interfaces;

namespace FimCore.Services
{
    public class FimEngine
    {
        private readonly IHashCalculator _hashCalculator;
        private readonly IBaselineRepository _repository;

        public FimEngine(IHashCalculator hashCalculator, IBaselineRepository repository)
        {
            _hashCalculator = hashCalculator;
            _repository = repository;
        }
    }
}