// ============================================================
// MemberService.cs — Member Business Logic
// Library Management System — Business Logic Layer
// ============================================================

using LibraryManagementSystem.DataAccess;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.BusinessLogic
{
    /// <summary>
    /// Service class that handles member business rules and validation.
    /// </summary>
    public class MemberService
    {
        private readonly MemberRepository _repository = new();

        /// <summary>Gets all members.</summary>
        public List<Member> GetAllMembers() => _repository.GetAll();

        /// <summary>Gets a member by ID.</summary>
        public Member? GetMemberById(int memberId) => _repository.GetById(memberId);

        /// <summary>Searches members.</summary>
        public List<Member> SearchMembers(string searchTerm) =>
            _repository.Search(searchTerm);

        /// <summary>Gets total member count.</summary>
        public int GetTotalCount() => _repository.GetTotalCount();

        /// <summary>Gets the number of books a member currently has.</summary>
        public int GetBorrowCount(int memberId) => _repository.GetBorrowCount(memberId);

        /// <summary>
        /// Adds a new member with validation.
        /// </summary>
        public (bool Success, string Message) AddMember(Member member)
        {
            try
            {
                // Validate member data
                var errors = member.Validate();
                if (errors.Count > 0)
                    return (false, string.Join("\n", errors));

                // Set registration date to today
                member.RegistrationDate = DateTime.Now;

                // Add to database
                int newId = _repository.Add(member);
                return (true, $"Member added successfully! (ID: {newId})");
            }
            catch (Exception ex)
            {
                return (false, $"Error adding member: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing member with validation.
        /// </summary>
        public (bool Success, string Message) UpdateMember(Member member)
        {
            try
            {
                var errors = member.Validate();
                if (errors.Count > 0)
                    return (false, string.Join("\n", errors));

                _repository.Update(member);
                return (true, "Member updated successfully!");
            }
            catch (Exception ex)
            {
                return (false, $"Error updating member: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a member by ID.
        /// Prevents deletion if the member has active borrowed books.
        /// </summary>
        public (bool Success, string Message) DeleteMember(int memberId)
        {
            try
            {
                // Check if member has active issues
                if (_repository.HasActiveIssues(memberId))
                    return (false, "Cannot delete member. They have unreturned books.");

                _repository.Delete(memberId);
                return (true, "Member deleted successfully!");
            }
            catch (Exception ex)
            {
                return (false, $"Error deleting member: {ex.Message}");
            }
        }
    }
}
