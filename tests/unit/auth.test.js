'use strict';

const jwt = require('jsonwebtoken');
const { authenticateToken } = require('../../src/middleware/auth');

const SECRET = 'test_secret';

function buildReqResNext({ token, secret } = {}) {
  process.env.JWT_SECRET = secret !== undefined ? secret : SECRET;

  const authHeader = token !== undefined ? `Bearer ${token}` : undefined;
  const req = { headers: authHeader ? { authorization: authHeader } : {} };
  const res = {
    status: jest.fn().mockReturnThis(),
    json: jest.fn().mockReturnThis(),
  };
  const next = jest.fn();
  return { req, res, next };
}

afterEach(() => {
  delete process.env.JWT_SECRET;
});

describe('authenticateToken middleware', () => {
  test('llama next() con token válido y adjunta payload a req.user', () => {
    const payload = { id: 1, username: 'admin' };
    const token = jwt.sign(payload, SECRET, { expiresIn: '1h' });
    const { req, res, next } = buildReqResNext({ token });

    authenticateToken(req, res, next);

    expect(next).toHaveBeenCalledTimes(1);
    expect(req.user).toMatchObject(payload);
    expect(res.status).not.toHaveBeenCalled();
  });

  test('retorna 401 cuando no hay header Authorization', () => {
    const { req, res, next } = buildReqResNext({});

    authenticateToken(req, res, next);

    expect(res.status).toHaveBeenCalledWith(401);
    expect(res.json).toHaveBeenCalledWith(
      expect.objectContaining({ error: 'Unauthorized' })
    );
    expect(next).not.toHaveBeenCalled();
  });

  test('retorna 403 cuando el token está expirado', () => {
    const token = jwt.sign({ id: 1 }, SECRET, { expiresIn: '-1s' });
    const { req, res, next } = buildReqResNext({ token });

    authenticateToken(req, res, next);

    expect(res.status).toHaveBeenCalledWith(403);
    expect(res.json).toHaveBeenCalledWith(
      expect.objectContaining({ message: 'El token ha expirado.' })
    );
    expect(next).not.toHaveBeenCalled();
  });

  test('retorna 403 cuando el token tiene firma inválida', () => {
    const token = jwt.sign({ id: 1 }, 'wrong_secret', { expiresIn: '1h' });
    const { req, res, next } = buildReqResNext({ token });

    authenticateToken(req, res, next);

    expect(res.status).toHaveBeenCalledWith(403);
    expect(res.json).toHaveBeenCalledWith(
      expect.objectContaining({ message: 'Token inválido.' })
    );
    expect(next).not.toHaveBeenCalled();
  });

  test('retorna 403 cuando el token está malformado', () => {
    const { req, res, next } = buildReqResNext({ token: 'not.a.valid.jwt' });

    authenticateToken(req, res, next);

    expect(res.status).toHaveBeenCalledWith(403);
    expect(next).not.toHaveBeenCalled();
  });

  test('retorna 500 cuando JWT_SECRET no está configurado', () => {
    const token = jwt.sign({ id: 1 }, SECRET, { expiresIn: '1h' });
    const { req, res, next } = buildReqResNext({ token, secret: '' });

    authenticateToken(req, res, next);

    expect(res.status).toHaveBeenCalledWith(500);
    expect(next).not.toHaveBeenCalled();
  });
});
