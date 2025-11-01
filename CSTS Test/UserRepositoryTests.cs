using CSTS.Data;
using CSTS.DTOs.UserDTO;
using CSTS.Models;
using CSTS.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CSTS.Tests.Repositories
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private CstsDbContext _context;
        private UserRepository _userRepo;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CstsDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // isolated DB per test
                .Options;

            _context = new CstsDbContext(options);
            _userRepo = new UserRepository(_context);
        }

        [TearDown]
        public void Teardown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

      
        [Test]
        public async Task RegisterAsync_ShouldRegisterNewUser()
        {
            var dto = new RegisterUserDTO
            {
                Name = "John Doe",
                Email = "john@example.com",
                Password = "password123",
                Role = "Customer"
            };

            var result = await _userRepo.RegisterAsync(dto);

            Assert.IsNotNull(result);
            Assert.That(result.Email, Is.EqualTo("john@example.com"));
            Assert.That(result.Role, Is.EqualTo("Customer"));
            Assert.That(result.IsActive, Is.True);
        }

  
        [Test]
        public async Task RegisterAsync_ShouldFailForDuplicateEmail()
        {
            var dto = new RegisterUserDTO
            {
                Name = "Alice",
                Email = "alice@example.com",
                Password = "pass123",
                Role = "Agent"
            };

            await _userRepo.RegisterAsync(dto);
            var result = await _userRepo.RegisterAsync(dto);

            Assert.IsNull(result);
        }

      
        [Test]
        public async Task LoginAsync_ShouldReturnUser_WhenCredentialsMatch()
        {
            var dto = new RegisterUserDTO
            {
                Name = "Admin",
                Email = "admin@csts.com",
                Password = "secret",
                Role = "Admin"
            };
            await _userRepo.RegisterAsync(dto);

            var loginDto = new LoginUserDTO
            {
                Email = "admin@csts.com",
                Password = "secret"
            };

            var result = await _userRepo.LoginAsync(loginDto);

            Assert.IsNotNull(result);
            Assert.That(result.Email, Is.EqualTo("admin@csts.com"));
        }

       
        [Test]
        public async Task LoginAsync_ShouldReturnNull_ForWrongPassword()
        {
            var dto = new RegisterUserDTO
            {
                Name = "User1",
                Email = "user1@csts.com",
                Password = "password",
                Role = "Customer"
            };
            await _userRepo.RegisterAsync(dto);

            var loginDto = new LoginUserDTO
            {
                Email = "user1@csts.com",
                Password = "wrongpassword"
            };

            var result = await _userRepo.LoginAsync(loginDto);

            Assert.IsNull(result);
        }

       
        [Test]
        public async Task GetUsersByRoleAsync_ShouldReturnAgentsOnly()
        {
            await _userRepo.RegisterAsync(new RegisterUserDTO { Name = "Agent1", Email = "a1@csts.com", Password = "a1", Role = "Agent" });
            await _userRepo.RegisterAsync(new RegisterUserDTO { Name = "Customer1", Email = "c1@csts.com", Password = "c1", Role = "Customer" });

            var agents = await _userRepo.GetUsersByRoleAsync(UserRole.Agent);

            Assert.That(agents.Count(), Is.EqualTo(1));
            Assert.That(agents.First().Role, Is.EqualTo("Agent"));
        }

      
        [Test]
        public async Task UpdateUserStatusAsync_ShouldActivateAndDeactivate()
        {
            var dto = new RegisterUserDTO
            {
                Name = "Sam",
                Email = "sam@csts.com",
                Password = "123",
                Role = "Customer"
            };
            var user = await _userRepo.RegisterAsync(dto);

          
            var deactivateResult = await _userRepo.UpdateUserStatusAsync(user.UserId, false);
            Assert.IsTrue(deactivateResult);

            var updatedUser = await _context.Users.FindAsync(user.UserId);
            Assert.IsFalse(updatedUser!.IsActive);

            
            var activateResult = await _userRepo.UpdateUserStatusAsync(user.UserId, true);
            Assert.IsTrue(activateResult);

            updatedUser = await _context.Users.FindAsync(user.UserId);
            Assert.IsTrue(updatedUser!.IsActive);
        }

        
        [Test]
        public async Task DeleteUserByRoleAsync_ShouldDeleteAgent()
        {
            var dto = new RegisterUserDTO
            {
                Name = "AgentDel",
                Email = "agentdel@csts.com",
                Password = "agent123",
                Role = "Agent"
            };

            var user = await _userRepo.RegisterAsync(dto);
            var result = await _userRepo.DeleteUserByRoleAsync(user.UserId, UserRole.Agent);

            Assert.IsTrue(result);
            Assert.That(_context.Users.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task DeleteUserByRoleAsync_ShouldFailForWrongRole()
        {
            var dto = new RegisterUserDTO
            {
                Name = "WrongRoleUser",
                Email = "wrongrole@csts.com",
                Password = "wrong",
                Role = "Customer"
            };

            var user = await _userRepo.RegisterAsync(dto);
            var result = await _userRepo.DeleteUserByRoleAsync(user.UserId, UserRole.Agent);

            Assert.IsFalse(result);
        }
    }
}
