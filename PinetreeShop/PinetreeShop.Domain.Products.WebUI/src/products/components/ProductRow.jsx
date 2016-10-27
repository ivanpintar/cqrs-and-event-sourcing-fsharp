import React from 'react';

const ProductRow = (product, clickToEdit) => (
    <tr key={product.id}>
        <td>{product.name}</td>
        <td>{product.price}</td>
        <td onClick={clickToEdit}>{product.quantity}</td>
        <td>{product.quantity}</td>
    </tr>
);

export default ProductRow;