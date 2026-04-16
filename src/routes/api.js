'use strict';

const express = require('express');
const rateLimit = require('express-rate-limit');
const { authenticateToken } = require('../middleware/auth');

const router = express.Router();

const apiLimiter = rateLimit({
  windowMs: 15 * 60 * 1000, // 15 minutos
  max: 100,
  standardHeaders: true,
  legacyHeaders: false,
  message: { error: 'Too Many Requests', message: 'Demasiadas peticiones, intenta más tarde.' },
});

/**
 * GET /api/public
 * Endpoint público: no requiere autenticación.
 */
router.get('/public', (req, res) => {
  res.status(200).json({ message: 'Este recurso es público.' });
});

/**
 * GET /api/profile
 * Endpoint protegido: requiere un JWT válido.
 */
router.get('/profile', apiLimiter, authenticateToken, (req, res) => {
  res.status(200).json({
    message: 'Acceso concedido.',
    user: req.user,
  });
});

/**
 * GET /api/data
 * Endpoint protegido: requiere un JWT válido.
 */
router.get('/data', apiLimiter, authenticateToken, (req, res) => {
  res.status(200).json({
    message: 'Datos confidenciales.',
    data: [{ id: 1, valor: 'secreto' }],
  });
});

module.exports = router;
