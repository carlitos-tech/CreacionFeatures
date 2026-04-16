'use strict';

process.env.JWT_SECRET = 'integration_test_secret';
process.env.JWT_EXPIRES_IN = '1h';
process.env.DEMO_USERNAME = 'admin';
process.env.DEMO_PASSWORD = 'secret';

const request = require('supertest');
const jwt = require('jsonwebtoken');
const app = require('../../src/app');

const SECRET = process.env.JWT_SECRET;

function validToken(payload = { id: 1, username: 'admin' }) {
  return jwt.sign(payload, SECRET, { expiresIn: '1h' });
}

function expiredToken() {
  return jwt.sign({ id: 1 }, SECRET, { expiresIn: '-1s' });
}

describe('POST /auth/login', () => {
  test('retorna 200 y un token con credenciales válidas', async () => {
    const res = await request(app)
      .post('/auth/login')
      .send({ username: 'admin', password: 'secret' });

    expect(res.statusCode).toBe(200);
    expect(res.body).toHaveProperty('token');
    const decoded = jwt.verify(res.body.token, SECRET);
    expect(decoded).toMatchObject({ username: 'admin' });
  });

  test('retorna 401 con credenciales inválidas', async () => {
    const res = await request(app)
      .post('/auth/login')
      .send({ username: 'admin', password: 'wrong' });

    expect(res.statusCode).toBe(401);
    expect(res.body).toMatchObject({ error: 'Unauthorized' });
  });

  test('retorna 400 cuando faltan credenciales', async () => {
    const res = await request(app)
      .post('/auth/login')
      .send({ username: 'admin' });

    expect(res.statusCode).toBe(400);
    expect(res.body).toMatchObject({ error: 'Bad Request' });
  });
});

describe('GET /api/public', () => {
  test('retorna 200 sin token', async () => {
    const res = await request(app).get('/api/public');
    expect(res.statusCode).toBe(200);
    expect(res.body).toHaveProperty('message');
  });
});

describe('GET /api/profile (endpoint protegido)', () => {
  test('retorna 200 con token válido', async () => {
    const res = await request(app)
      .get('/api/profile')
      .set('Authorization', `Bearer ${validToken()}`);

    expect(res.statusCode).toBe(200);
    expect(res.body).toMatchObject({ message: 'Acceso concedido.' });
    expect(res.body.user).toMatchObject({ username: 'admin' });
  });

  test('retorna 401 sin token', async () => {
    const res = await request(app).get('/api/profile');
    expect(res.statusCode).toBe(401);
    expect(res.body).toMatchObject({ error: 'Unauthorized' });
  });

  test('retorna 403 con token expirado', async () => {
    const res = await request(app)
      .get('/api/profile')
      .set('Authorization', `Bearer ${expiredToken()}`);

    expect(res.statusCode).toBe(403);
    expect(res.body).toMatchObject({ message: 'El token ha expirado.' });
  });

  test('retorna 403 con token inválido', async () => {
    const res = await request(app)
      .get('/api/profile')
      .set('Authorization', 'Bearer este.no.es.valido');

    expect(res.statusCode).toBe(403);
    expect(res.body).toMatchObject({ error: 'Forbidden' });
  });
});

describe('GET /api/data (endpoint protegido)', () => {
  test('retorna 200 con token válido', async () => {
    const res = await request(app)
      .get('/api/data')
      .set('Authorization', `Bearer ${validToken()}`);

    expect(res.statusCode).toBe(200);
    expect(res.body).toHaveProperty('data');
  });

  test('retorna 401 sin token', async () => {
    const res = await request(app).get('/api/data');
    expect(res.statusCode).toBe(401);
  });
});

describe('Ruta no encontrada', () => {
  test('retorna 404 para rutas inexistentes', async () => {
    const res = await request(app).get('/no/existe');
    expect(res.statusCode).toBe(404);
  });
});
