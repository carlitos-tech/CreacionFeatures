'use strict';

require('dotenv').config();

const express = require('express');
const authRouter = require('./routes/auth');
const apiRouter = require('./routes/api');

const app = express();

app.use(express.json());

// Rutas de autenticación (públicas)
app.use('/auth', authRouter);

// Rutas de la API (algunas protegidas por JWT)
app.use('/api', apiRouter);

// Manejador de rutas no encontradas
app.use((req, res) => {
  res.status(404).json({ error: 'Not Found', message: 'Recurso no encontrado.' });
});

module.exports = app;
