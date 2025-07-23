using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Core.Interfaces;

namespace Users.Application.Commands.CreateUserRating
{
    public class CreateUserRatingCommandHandler : IRequestHandler<CreateUserRatingCommand, bool>
    {
        private readonly IUserRatingRepository _userRatingRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserRatingCommandHandler(
            IUserRatingRepository userRatingRepository,
            IUnitOfWork unitOfWork)
        {
            _userRatingRepository = userRatingRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(CreateUserRatingCommand request, CancellationToken cancellationToken)
        {
            var userRating = new UserRating
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Rating = 3, // Default rating as integer
                NumOfReviews = 0, // Initial number of reviews
                CreatedAt = DateTime.UtcNow
            };

            await _userRatingRepository.AddAsync(userRating);
            await _unitOfWork.SaveChangesAsync();
            
            return true;
        }
    }
} 