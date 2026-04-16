'use strict';

const jwt = require('jsonwebtoken');

/**
 * Middleware de validación JWT.
 * Extrae el token del header Authorization (Bearer <token>),
 * verifica su firma y expiración, y adjunta el payload al request.
 * Devuelve 401 si el token está ausente y 403 si es inválido o expirado.
 */
function authenticateToken(req, res, next) {
  const authHeader = req.headers['authorization'];
  const token = authHeader && authHeader.startsWith('Bearer ')
    ? authHeader.slice(7)
    : null;

  if (!token) {
    return res.status(401).json({
      error: 'Unauthorized',
      message: 'Token de acceso requerido.',
    });
  }

  const secret = process.env.JWT_SECRET;
  if (!secret) {
    return res.status(500).json({
      error: 'Internal Server Error',
      message: 'Configuración del servidor incorrecta.',
    });
  }

  jwt.verify(token, secret, (err, payload) => {
    if (err) {
      if (err.name === 'TokenExpiredError') {
        return res.status(403).json({
          error: 'Forbidden',
          message: 'El token ha expirado.',
        });
      }
      return res.status(403).json({
        error: 'Forbidden',
        message: 'Token inválido.',
      });
    }

    req.user = payload;
    next();
  });
}

module.exports = { authenticateToken };
