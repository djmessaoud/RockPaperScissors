--
-- PostgreSQL database dump
--

-- Dumped from database version 16.2
-- Dumped by pg_dump version 16.2

-- Started on 2024-03-18 03:40:34

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 220 (class 1259 OID 16433)
-- Name: gametransactions; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.gametransactions (
    "transactionId" integer NOT NULL,
    "fromUserId" integer NOT NULL,
    "toUserId" integer NOT NULL,
    amount numeric(7,2) NOT NULL,
    "timestamp" timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


ALTER TABLE public.gametransactions OWNER TO postgres;

--
-- TOC entry 219 (class 1259 OID 16432)
-- Name: gametransactions_transactionid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.gametransactions_transactionid_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.gametransactions_transactionid_seq OWNER TO postgres;

--
-- TOC entry 4868 (class 0 OID 0)
-- Dependencies: 219
-- Name: gametransactions_transactionid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.gametransactions_transactionid_seq OWNED BY public.gametransactions."transactionId";


--
-- TOC entry 218 (class 1259 OID 16410)
-- Name: matchhistory; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.matchhistory (
    "matchId" integer NOT NULL,
    "playerOneId" integer,
    "playerTwoId" integer,
    "winnerId" integer,
    stake numeric(10,2) NOT NULL,
    "timestamp" timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "playerOneChoice" integer,
    "playerTwoChoice" integer,
    "GameState" integer
);


ALTER TABLE public.matchhistory OWNER TO postgres;

--
-- TOC entry 217 (class 1259 OID 16409)
-- Name: matchhistory_matchid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.matchhistory_matchid_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.matchhistory_matchid_seq OWNER TO postgres;

--
-- TOC entry 4869 (class 0 OID 0)
-- Dependencies: 217
-- Name: matchhistory_matchid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.matchhistory_matchid_seq OWNED BY public.matchhistory."matchId";


--
-- TOC entry 216 (class 1259 OID 16401)
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.users (
    userid integer NOT NULL,
    username character varying(255) NOT NULL,
    balance numeric(7,2) NOT NULL
);


ALTER TABLE public.users OWNER TO postgres;

--
-- TOC entry 215 (class 1259 OID 16400)
-- Name: users_userid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.users_userid_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.users_userid_seq OWNER TO postgres;

--
-- TOC entry 4870 (class 0 OID 0)
-- Dependencies: 215
-- Name: users_userid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.users_userid_seq OWNED BY public.users.userid;


--
-- TOC entry 4701 (class 2604 OID 16436)
-- Name: gametransactions transactionId; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.gametransactions ALTER COLUMN "transactionId" SET DEFAULT nextval('public.gametransactions_transactionid_seq'::regclass);


--
-- TOC entry 4699 (class 2604 OID 16413)
-- Name: matchhistory matchId; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.matchhistory ALTER COLUMN "matchId" SET DEFAULT nextval('public.matchhistory_matchid_seq'::regclass);


--
-- TOC entry 4698 (class 2604 OID 16404)
-- Name: users userid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users ALTER COLUMN userid SET DEFAULT nextval('public.users_userid_seq'::regclass);


--
-- TOC entry 4712 (class 2606 OID 16439)
-- Name: gametransactions gametransactions_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.gametransactions
    ADD CONSTRAINT gametransactions_pkey PRIMARY KEY ("transactionId");


--
-- TOC entry 4710 (class 2606 OID 16416)
-- Name: matchhistory matchhistory_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.matchhistory
    ADD CONSTRAINT matchhistory_pkey PRIMARY KEY ("matchId");


--
-- TOC entry 4704 (class 2606 OID 16406)
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (userid);


--
-- TOC entry 4706 (class 2606 OID 16408)
-- Name: users users_username_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_username_key UNIQUE (username);


--
-- TOC entry 4713 (class 1259 OID 16457)
-- Name: idx_gametransactions_fromuser; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_gametransactions_fromuser ON public.gametransactions USING btree ("fromUserId");


--
-- TOC entry 4714 (class 1259 OID 16458)
-- Name: idx_gametransactions_touser; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_gametransactions_touser ON public.gametransactions USING btree ("toUserId");


--
-- TOC entry 4707 (class 1259 OID 16455)
-- Name: idx_match_history_player_one; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_match_history_player_one ON public.matchhistory USING btree ("playerOneId");


--
-- TOC entry 4708 (class 1259 OID 16456)
-- Name: idx_match_history_player_two; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_match_history_player_two ON public.matchhistory USING btree ("playerTwoId");


--
-- TOC entry 4718 (class 2606 OID 16440)
-- Name: gametransactions gametransactions_fromuserid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.gametransactions
    ADD CONSTRAINT gametransactions_fromuserid_fkey FOREIGN KEY ("fromUserId") REFERENCES public.users(userid);


--
-- TOC entry 4719 (class 2606 OID 16445)
-- Name: gametransactions gametransactions_touserid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.gametransactions
    ADD CONSTRAINT gametransactions_touserid_fkey FOREIGN KEY ("toUserId") REFERENCES public.users(userid);


--
-- TOC entry 4715 (class 2606 OID 16417)
-- Name: matchhistory matchhistory_playeroneid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.matchhistory
    ADD CONSTRAINT matchhistory_playeroneid_fkey FOREIGN KEY ("playerOneId") REFERENCES public.users(userid);


--
-- TOC entry 4716 (class 2606 OID 16422)
-- Name: matchhistory matchhistory_playertwoid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.matchhistory
    ADD CONSTRAINT matchhistory_playertwoid_fkey FOREIGN KEY ("playerTwoId") REFERENCES public.users(userid);


--
-- TOC entry 4717 (class 2606 OID 16427)
-- Name: matchhistory matchhistory_winnerid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.matchhistory
    ADD CONSTRAINT matchhistory_winnerid_fkey FOREIGN KEY ("winnerId") REFERENCES public.users(userid);


-- Completed on 2024-03-18 03:40:34

--
-- PostgreSQL database dump complete
--

