import express from "express";
import {urlencoded} from "body-parser";
import {send} from "./index";

const app = express();
app.use(urlencoded({extended: false}));
app.listen(1337);
