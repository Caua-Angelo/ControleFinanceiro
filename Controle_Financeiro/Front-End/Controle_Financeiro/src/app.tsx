import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Inicial from './Pages/index.tsx';
import Categoria from './Pages/Categoria.tsx';
import Usuario from './Pages/Usuario.tsx';
import Transacao from './Pages/Transacao.tsx';
import Layout from './Components/layout.tsx'

function app() {
  return (
    <Router>
      <Routes>
        {/* Layout que aparece tem todas  as outras telas */}
        <Route path="/" element={<Layout />}>
          <Route index element={<Inicial />} />
          <Route path="categoria" element={<Categoria />} />
          <Route path="transacao" element={<Transacao />} />
          <Route path="usuario" element={<Usuario />} />
        </Route>
      </Routes>
    </Router>
  );
}

export default app;