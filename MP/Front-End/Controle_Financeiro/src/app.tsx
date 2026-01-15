import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Login from './Pages/Hub';
import Categoria from './Pages/Categoria';
import Usuario from './Pages/Usuario';
import Transacao from './Pages/Transacao';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Login />} />
        <Route path="/categoria" element={<Categoria />} />
        <Route path="/transacao" element={<Transacao />} />
        <Route path="/usuario" element={<Usuario />} />
      </Routes>
    </Router>
  );
}

export default App;