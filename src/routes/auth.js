'use strict';

const express = require('express');
const jwt = require('jsonwebtoken');
const rateLimit = require('express-rate-limit');

const router = express.Router();

const loginLimiter = rateLimit({
  windowMs: 15 * 60 * 1000, // 15 minutos
  max: 10,
  standardHeaders: true,
  legacyHeaders: false,
  message: { error: 'Too Many Requests', message: 'Demasiados intentos de login, intenta más tarde.' },
});

/**
 * POST /auth/login
 * Recibe { username, password } y retorna un JWT si las credenciales son válidas.
 * En un proyecto real, aquí se consultaría la base de datos y se validaría el hash de la contraseña.
 */
router.post('/login', loginLimiter, (req, res) => {
  const { username, password } = req.body;

  if (!username || !password) {
    return res.status(400).json({
      error: 'Bad Request',
      message: 'Se requieren username y password.',
    });
  }

  // Demo: credenciales de ejemplo gestionadas mediante variables de entorno.
  // En producción reemplazar con consulta a base de datos y comparación de hash.
  const demoUsername = process.env.DEMO_USERNAME;
  const demoPassword = process.env.DEMO_PASSWORD;

  if (!demoUsername || !demoPassword) {
    return res.status(500).json({
      error: 'Internal Server Error',
      message: 'Configuración del servidor incorrecta.',
    });
  }

  if (username !== demoUsername || password !== demoPassword) {
    return res.status(401).json({
      error: 'Unauthorized',
      message: 'Credenciales inválidas.',
    });
  }

  const secret = process.env.JWT_SECRET;
  if (!secret) {
    return res.status(500).json({
      error: 'Internal Server Error',
      message: 'Configuración del servidor incorrecta.',
    });
  }
  const expiresIn = process.env.JWT_EXPIRES_IN || '1h';

  const token = jwt.sign(
    { id: 1, username: demoUsername },
    secret,
    { expiresIn }
  );

  return res.status(200).json({ token });
});

module.exports = router;
