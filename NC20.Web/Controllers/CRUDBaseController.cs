﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NC20.Repository;
using NC20.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NC20.Web.Controllers
{
    public abstract class CRUDBaseController<TModel, TEntity> : BaseController where TEntity : class, new()
    {
        public CRUDBaseController(NC20DBRepository repository) : base(repository) { }
        public abstract IBaseRepository<TEntity> GetRepository();
        public abstract TModel Map(TEntity entity);
        public abstract TEntity Bind(TEntity entity, TModel model);
        public abstract TEntity GetEntity(IBaseRepository<TEntity> repository, int id);
        public abstract int GetModelId(TModel model);
        protected virtual string ObjectValidate(TModel model)
        {
            return string.Empty;
        }
        protected virtual IQueryable<TEntity> FilterQuery(IQueryable<TEntity> query, GetGridRequestModel request)
        {
            return query;
        }

        [HttpPost, Route("filter")]
        [AllowAnonymous]
        public GetGridResponseModel<TModel> Filter([FromBody]GetGridRequestModel request)
        {
            var query = this.GetRepository().GetAll();
            query = this.FilterQuery(query, request);
            // count
            var totalRecord = query.Count();

            // take
            var entities = query.Skip((request.CurrentPage - 1) * request.PageSize).Take(request.PageSize).ToList();

            //map
            var models = entities.Select(Map).ToList();

            var response = new GetGridResponseModel<TModel>();
            response.DataSource = models;
            response.TotalRecord = totalRecord;
            return response;
        }

        [HttpGet, Route("get")]
        [AllowAnonymous]
        public virtual TModel Get(int id)
        {
            var entity = this.GetEntity(this.GetRepository(), id);
            return Map(entity);
        }

        [HttpPost, Route("update-or-create")]
        public ObjectSavedResponseModel<TModel> UpdateOrCreate([FromBody]TModel model)
        {
            var entity = this.GetEntity(this.GetRepository(), this.GetModelId(model));
            if (entity == null)
            {
                if (this.GetModelId(model) == 0)
                {
                    entity = new TEntity();
                }
                else
                {
                    return new ObjectSavedResponseModel<TModel>()
                    {
                        IsSuccess = false,
                        ErrorCode = "UpdateNotFoundEntity",
                        Model = default(TModel)
                    };
                }
            }
            var validateResult = ObjectValidate(model);
            if (!string.IsNullOrEmpty(validateResult))
            {
                return new ObjectSavedResponseModel<TModel>()
                {
                    IsSuccess = false,
                    ErrorCode = validateResult,
                    ErrorMessage = validateResult,
                    Model = default(TModel)
                };
            }
            entity = this.Bind(entity, model);
            this.GetRepository().Save(entity);
            this.NC20DBRepository.Commit();
            entity = GetEntity(GetRepository(), GetModelId(Map(entity)));
            return new ObjectSavedResponseModel<TModel>()
            {
                IsSuccess = true,
                ErrorCode = string.Empty,
                ErrorMessage = string.Empty,
                Model = Map(entity)
            };
        }
        [HttpPost, Route("create")]
        public TModel Create([FromBody]TModel model)
        {
            var entity = new TEntity();
            entity = Bind(entity, model);
            GetRepository().Save(entity);
            this.NC20DBRepository.Commit();
            return Map(entity);
        }
        [HttpPost, Route("delete")]
        public bool Delete([FromBody]int id)
        {
            var entity = GetRepository().GetObject(id);
            if (entity == null) return false;
            GetRepository().Delete(entity);
            this.NC20DBRepository.Commit();
            return true;
        }
    }
}