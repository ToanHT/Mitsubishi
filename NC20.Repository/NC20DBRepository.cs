﻿using NC20.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace NC20.Repository
{
    public class NC20DBUnitOfWork : BaseUnitOfWork<NC20Entities>
    {
        public NC20DBUnitOfWork(NC20Entities entity)
            : base(entity) { }

    }

    public class NC20DBRepository
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public IBaseUnitOfWork<NC20Entities> CBDBUnitOfWork { get; private set; }
        public NC20DBRepository(IBaseUnitOfWork<NC20Entities> unitOfWork, IServiceProvider serviceProvider)
        {
            this.CBDBUnitOfWork = unitOfWork;
            this.ServiceProvider = serviceProvider;
        }
        public void Commit()
        {
            this.CBDBUnitOfWork.Commit();
        }
        #region Repositories
        private IClaimRepository _ClaimRepository;
        public IClaimRepository ClaimRepository
        {
            get
            {
                return _ClaimRepository ??
                (_ClaimRepository = ServiceProvider.GetService<IClaimRepository>());
            }
        }
        private IImageRepository _ImageRepository;
        public IImageRepository ImageRepository
        {
            get
            {
                return _ImageRepository ??
                (_ImageRepository = ServiceProvider.GetService<IImageRepository>());
            }
        }
        private IRoleRepository _RoleRepository;
        public IRoleRepository RoleRepository
        {
            get
            {
                return _RoleRepository ??
                (_RoleRepository = ServiceProvider.GetService<IRoleRepository>());
            }
        }
        private IRoleClaimRepository _RoleClaimRepository;
        public IRoleClaimRepository RoleClaimRepository
        {
            get
            {
                return _RoleClaimRepository ??
                (_RoleClaimRepository = ServiceProvider.GetService<IRoleClaimRepository>());
            }
        }
        private IUserRepository _UserRepository;
        public IUserRepository UserRepository
        {
            get
            {
                return _UserRepository ??
                (_UserRepository = ServiceProvider.GetService<IUserRepository>());
            }
        }
        private IUserLoginTokenRepository _UserLoginTokenRepository;
        public IUserLoginTokenRepository UserLoginTokenRepository
        {
            get
            {
                return _UserLoginTokenRepository ??
                (_UserLoginTokenRepository = ServiceProvider.GetService<IUserLoginTokenRepository>());
            }
        }
        private IUserProfileRepository _UserProfileRepository;
        public IUserProfileRepository UserProfileRepository
        {
            get
            {
                return _UserProfileRepository ??
                (_UserProfileRepository = ServiceProvider.GetService<IUserProfileRepository>());
            }
        }
        private IUserRoleRepository _UserRoleRepository;
        public IUserRoleRepository UserRoleRepository
        {
            get
            {
                return _UserRoleRepository ??
                (_UserRoleRepository = ServiceProvider.GetService<IUserRoleRepository>());
            }
        }
        #endregion
    }
    public static class RegisterServiceHelper
    {
        public static void RegisterRepository(IServiceCollection services)
        {
            services.AddScoped<IClaimRepository, ClaimRepository>();
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleClaimRepository, RoleClaimRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserLoginTokenRepository, UserLoginTokenRepository>();
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        }
    }
}
