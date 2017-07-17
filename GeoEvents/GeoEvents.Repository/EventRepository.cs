﻿using GeoEvents.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoEvents.Common;
using GeoEvents.DAL;
using Npgsql;
using GeoEvents.Model.Common;
using AutoMapper;
using System.Data.Common;

namespace GeoEvents.Repository
{
    public class EventRepository : IEventRepository
    {
        #region Properties
        protected IPostgresConnection PostgresConn { get; private set; }

        protected IMapper Mapper { get; private set; }
        #endregion Properties

        #region Constructors
        public EventRepository(IPostgresConnection connection, IMapper mapper)
        {
            this.Mapper = mapper;
            this.PostgresConn = connection;
        }
        #endregion Constructors

        #region Methods

        /// <summary>
        /// Creates Event asynchronously.
        /// </summary>
        /// <param name="evt"></param>
        /// <returns>  
        /// return Created event (IEvent)
        /// </returns>
        public async Task<IEvent> CreateEventAsync(IEvent evt)
        {
            EventEntity evte = Mapper.Map<EventEntity>(evt);

            try

            {
                using (PostgresConn.CreateConnection())
                using (NpgsqlCommand commandInsert = new NpgsqlCommand(QueryHelper.GetInsertEventString(), PostgresConn.CreateConnection()))
                using (NpgsqlCommand commandSelect = new NpgsqlCommand(QueryHelper.GetSelectEventStringById(), PostgresConn.CreateConnection()))

                {
                    commandInsert.Parameters.AddWithValue(QueryHelper.ParId, NpgsqlTypes.NpgsqlDbType.Uuid, evte.Id);
                    commandInsert.Parameters.AddWithValue(QueryHelper.ParCategory, NpgsqlTypes.NpgsqlDbType.Integer, evte.Category);
                    commandInsert.Parameters.AddWithValue(QueryHelper.ParDescription, NpgsqlTypes.NpgsqlDbType.Text, evte.Description);
                    commandInsert.Parameters.AddWithValue(QueryHelper.ParStartTime, NpgsqlTypes.NpgsqlDbType.Timestamp, evte.StartTime);
                    commandInsert.Parameters.AddWithValue(QueryHelper.ParEndTime, NpgsqlTypes.NpgsqlDbType.Timestamp, evte.EndTime);
                    commandInsert.Parameters.AddWithValue(QueryHelper.ParLat, NpgsqlTypes.NpgsqlDbType.Double, evte.Lat);
                    commandInsert.Parameters.AddWithValue(QueryHelper.ParLong, NpgsqlTypes.NpgsqlDbType.Double, evte.Long);
                    commandInsert.Parameters.AddWithValue(QueryHelper.ParName, NpgsqlTypes.NpgsqlDbType.Text, evte.Name);
                    commandSelect.Parameters.AddWithValue(QueryHelper.ParEventId, NpgsqlTypes.NpgsqlDbType.Uuid, evte.Id);

                    await PostgresConn.CreateConnection().OpenAsync();
                    await commandInsert.ExecuteNonQueryAsync();

                    DbDataReader dr = await commandSelect.ExecuteReaderAsync();
                    while (dr.Read())
                    {
                        EventEntity evtR = new EventEntity
                        {
                            Id = new Guid(dr[0].ToString()),
                            StartTime = Convert.ToDateTime(dr[1]),
                            EndTime = Convert.ToDateTime(dr[2]),
                            Lat = Convert.ToDecimal(dr[3]),
                            Long = Convert.ToDecimal(dr[4]),
                            Name = dr[5].ToString(),
                            Description = dr[6].ToString(),
                            Category = Convert.ToInt32(dr[7])
                        };
                    }


                };
            }
            catch (NpgsqlException ex)
            {
                throw ex;
            }

            return Mapper.Map<IEvent>(evte);


        }

        /// <summary>
        /// Gets filtered Events asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>
        /// list of Events.
        /// </returns>
        public async Task<IEnumerable<IEvent>> GetEventsAsync(IFilter filter)
        {
            EventEntity tmp;
            List<IEvent> SelectEvents = new List<IEvent>();
            try
            {
                using (PostgresConn.CreateConnection())
                using (NpgsqlCommand command = new NpgsqlCommand(QueryHelper.GetSelectEventString(filter), PostgresConn.CreateConnection()))
                {

                    SetParametersSearchEvents(filter, command);

                    await PostgresConn.CreateConnection().OpenAsync();
                    DbDataReader dr = await command.ExecuteReaderAsync();

                    while (dr.Read())
                    {
                        tmp = new EventEntity
                        {
                            Id = new Guid(dr[0].ToString()),
                            StartTime = Convert.ToDateTime(dr[1]),
                            EndTime = Convert.ToDateTime(dr[2]),
                            Lat = Convert.ToDecimal(dr[3]),
                            Long = Convert.ToDecimal(dr[4]),
                            Name = dr[5].ToString(),
                            Description = dr[6].ToString(),
                            Category = Convert.ToInt32(dr[7])
                        };

                        SelectEvents.Add(Mapper.Map<IEvent>(tmp));
                    }


                }
            }
            catch (NpgsqlException ex)
            {
                throw ex;
            }
            return SelectEvents;

        }


        /// <summary>
        /// Get event count asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>
        /// number of events .
        /// </returns>
        public async Task<Int64> GetEventCountAsync(IFilter filter)
        {
            Int64 Count;
            try
            {
                using (PostgresConn.CreateConnection())
                using (NpgsqlCommand command = new NpgsqlCommand(QueryHelper.GetSelectCountEventString(filter), PostgresConn.CreateConnection()))
                {

                    SetParametersSearchEvents(filter, command);

                    await PostgresConn.CreateConnection().OpenAsync();
                    object dr = await command.ExecuteScalarAsync();

                    Count = Convert.ToInt64(dr);

                }
            }
            catch (NpgsqlException ex)
            {
                throw ex;
            }



            return Count;

        }

        /// <summary>
        /// Sets Parameters of NpgsqlCommand by Filter
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="command"></param>
        private void SetParametersSearchEvents(IFilter filter,NpgsqlCommand command)
        {

            if (filter.ULat != null) { command.Parameters.AddWithValue(QueryHelper.ParLat, NpgsqlTypes.NpgsqlDbType.Double, filter.ULat); }
            if (filter.ULong != null) { command.Parameters.AddWithValue(QueryHelper.ParLong, NpgsqlTypes.NpgsqlDbType.Double, filter.ULong); }
            if (filter.Radius != null) { command.Parameters.AddWithValue(QueryHelper.ParRadius, NpgsqlTypes.NpgsqlDbType.Double, filter.Radius * 1000); }
            if (filter.StartTime != null) { command.Parameters.AddWithValue(QueryHelper.ParUserStartTime, NpgsqlTypes.NpgsqlDbType.Timestamp, filter.StartTime); }
            if (filter.EndTime != null) { command.Parameters.AddWithValue(QueryHelper.ParUserEndTime, NpgsqlTypes.NpgsqlDbType.Timestamp, filter.EndTime); }
            if (filter.Category != null) { command.Parameters.AddWithValue(QueryHelper.ParCategory, NpgsqlTypes.NpgsqlDbType.Integer, filter.Category); }
            if (!string.IsNullOrWhiteSpace(filter.SearchString)) { command.Parameters.AddWithValue(QueryHelper.ParSearcString, NpgsqlTypes.NpgsqlDbType.Varchar, filter.SearchString); }

         
        }

        #endregion Methods
    }
}